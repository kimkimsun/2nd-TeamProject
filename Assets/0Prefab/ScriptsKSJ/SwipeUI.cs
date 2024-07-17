using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SungHwan
{
    public class SwipeUI : MonoBehaviour
    {
        // Scrollbar의 위치 바탕으로 현재 페이지 검사
        [SerializeField]
        private Scrollbar scrollbar;
        // 현재 페이지를 나타내는 Image Transform
        [SerializeField]
        private Transform[] circleContents;
        // 페이지가 Swipe 되는 시간
        [SerializeField]
        private float swipeTime = 0.2f;
        // Swipe를 위한 최소 이동 거리
        [SerializeField]
        private float swipeMinDistance = 50.0f;

        private float[] scrollPageValues;
        private float valueDistance = 0;

        private int currentPage = 0;
        private int maxPage = 0;

        private float startTouchX;
        private float endTouchX;

        private bool isSwipeMode = false;
        private float circleContentScale = 1.5f;

        private void Awake()
        {
            // 페이지의 각 value값을 저장하는 배열 메모리 할당
            scrollPageValues = new float[transform.childCount];

            // 페이지 간격
            valueDistance = 1f / (scrollPageValues.Length - 1f);

            // 각 페이지 value 위치 설정 [0 <= value <= 1]
            for (int i = 0; i < scrollPageValues.Length; i++)
            {
                scrollPageValues[i] = valueDistance * i;
            }

            // 최대 페이지의 수
            maxPage = transform.childCount;
        }
        void Start()
        {
            // 최초 시작시 0번 페이지 설정
            SetScrollBarValue(0);
        }

        public void SetScrollBarValue(int index)
        {
            currentPage = index;
            scrollbar.value = scrollPageValues[index];
        }

        void Update()
        {
            /// <summary>
            /// 본문, 하단 Swipe
            /// </summary>
            UpdateInput();
            UpdateCircleContent();
        }

        private void UpdateInput()
        {
            if (isSwipeMode == true)
                return;

#if UNITY_EDITOR
            // 좌클릭 때 1회
            // 터치 시작 지점 ( Swipe 방향 구분 )
            if (Input.GetMouseButtonDown(0))
            {
                startTouchX = Input.mousePosition.x;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                endTouchX = Input.mousePosition.x;
                UpdateSwipe();
            }
#endif
        }

        private void UpdateSwipe()
        {
            // 너무 작은 거리를 움직였을 때는 Swipe X
            // 원래 페이지로 Swipe해서 돌아감
            if (Mathf.Abs(startTouchX - endTouchX) < swipeMinDistance)
            {
                StartCoroutine(OnSwipeOneStep(currentPage));
                return;
            }
            // Swipe 방향
            bool isLeft = startTouchX < endTouchX ? true : false;

            // 이동방향이 왼쪽일 때
            // 현재 페이지가 왼쪽 끝이면 ? 종료 : 현재 페이지 -1
            // 이동방향이 오른쪽일 때
            // 현재 페이지가 오른쪽 끝이면 ? 종료 : 현재 페이지 +1
            if (isLeft == true)
            {
                if (currentPage == 0) return;
                currentPage--;
            }
            else
            {
                if (currentPage == maxPage - 1) return;
                currentPage++;
            }
            // currentIndex번째로 Swipe 이동
            StartCoroutine(OnSwipeOneStep(currentPage));
        }

        /// <summary>
        /// 한 장 옆으로 넘기는 Swipe 효과 재생
        /// </summary>
        private IEnumerator OnSwipeOneStep(int index)
        {
            float start = scrollbar.value;
            float current = 0;
            float percent = 0;

            isSwipeMode = true;

            while (percent < 1)
            {
                current += Time.deltaTime;
                percent = current / swipeTime;

                scrollbar.value = Mathf.Lerp(start, scrollPageValues[index], percent);

                yield return null;
            }

            isSwipeMode = false;
        }

        private void UpdateCircleContent()
        {
            /// <summary>
            /// 아래에 배치된 페이지 버튼 크기, 색상 제어
            /// ( 현재 머물고 있는 페이지의 버튼만 수정 )
            /// 페이지 절반 넘어가면 현재 페이지 원을 바꿈
            /// </summary>
            for (int i = 0; i < scrollPageValues.Length; ++i)
            {
                circleContents[i].localScale = Vector2.one;
                circleContents[i].GetComponent<Image>().color = Color.white;

                if (scrollbar.value < scrollPageValues[i] + (valueDistance / 2) &&
                     scrollbar.value > scrollPageValues[i] - (valueDistance / 2))
                {
                    circleContents[i].localScale = Vector2.one * circleContentScale;
                    circleContents[i].GetComponent<Image>().color = Color.black;
                }
            }
        }
    }
}

