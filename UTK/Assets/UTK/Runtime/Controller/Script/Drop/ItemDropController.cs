using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UTK.Runtime.Controller.Drop
{
    /// <summary>
    /// A controller with the ability to drop items to a specific location or range based on progress.
    /// </summary>
    public class ItemDropController : MonoBehaviour
    {
        [SerializeField]
        private int degreeOfProgress;

        [SerializeField]
        private List<ItemDropData> dropDatas;

        #region Event

        /// <summary>
        /// Called when item droped.
        /// </summary>
        public UnityEngine.Events.UnityAction<ItemDropData> OnItemDrop;

        #endregion

        /// <summary>
        /// Increase progress.
        /// </summary>
        public void IncreaseDegreeOfProgress(int value)
        {
            degreeOfProgress += value;
            ItemDrop();
        }

        protected void Start()
        {
            //Initial drop.
            ItemDrop();
        }

        protected void Update()
        {

        }

        #region Internal

        void ItemDrop()
        {
            var items = dropDatas.Where(t => !t.IsDrop && t.EvaluationValue <= degreeOfProgress);
            foreach (var i in items)
            {
                Manager.ResourceManager.Instance.LoadAsync(i.DataPath, (GameObject prefab) =>
                {
                    OnCompleteLoad(prefab, i);
                });
            }
        }

        void OnCompleteLoad(GameObject prefab, ItemDropData data)
        {
            data.dataObject = Instantiate(prefab, data.Position, data.Rotation);
            data.dataObject.transform.localScale = data.Scale;
            data.IsDrop = true;
            OnItemDrop?.Invoke(data);
        }

        #endregion
    }
}

