using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace UTK.Runtime.Controller.AI
{
    /// <summary>
    /// AI controller with the ability to move along a specified path.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class PathAgentController : MonoBehaviour
    {
        [SerializeField]
        private List<PathData> pathDatas;

        [SerializeField]
        private float goalJudgeRemainingDistance = 0.5f;

        private int destPoint = -1; //-1 == Flag that is the first move.
        private bool isIntervaling = false;

        #region Property

        public NavMeshAgent Agent { get; protected set; } = null;

        public bool IsStoped { get => Agent.isStopped; }

        #endregion

        #region Callback

        public UnityAction OnMoveComplete { get; set; }

        #endregion

        public void Resume()
        {
            Agent.isStopped = false;
        }

        public void Stop()
        {
            Agent.isStopped = true;
        }

        protected void Start()
        {
            Agent = GetComponent<NavMeshAgent>();
            Debug.Assert(Agent != null);

            Resume();
        }

        protected void Update()
        {
            if (!isIntervaling && !Agent.pathPending && Agent.remainingDistance < goalJudgeRemainingDistance)
            {
                if (destPoint != -1)
                {
                    OnMoveComplete?.Invoke();
                }

                StartCoroutine(GotoNextPoint());
            }
        }

        protected IEnumerator GotoNextPoint()
        {
            if (pathDatas.Count == 0)
            {
                yield break;
            }

            if (destPoint == -1)
            {
                destPoint = 0;
            }

            isIntervaling = true;
            yield return new WaitForSeconds(pathDatas[destPoint].Interval);

            Agent.destination = pathDatas[destPoint].Position;
            destPoint = (destPoint + 1) % pathDatas.Count;
            isIntervaling = false;
        }

    }

}
