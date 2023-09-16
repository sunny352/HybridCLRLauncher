using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Launcher.UI
{
    public class UILogo : MonoBehaviour
    {
        public Image[] images;
        public float duration = 1f;
        
        private bool _isFinished;
        public async Task Wait()
        {
            await UniTask.WaitUntil(() => _isFinished);
        }
        
        private float _startTime;
        private void Start()
        {
            foreach (var image in images)
            {
                image.enabled = false;
            }
            _startTime = Time.time;
        }
        
        private int _preIndex = -1;
        private void Update()
        {
            var index = Mathf.FloorToInt((Time.time - _startTime) / (duration * 2));
            if (_preIndex != index)
            {
                if (_preIndex >= 0 && _preIndex < images.Length)
                {
                    images[_preIndex].enabled = false;
                }
                _preIndex = index;
                if (index >= images.Length)
                {
                    _isFinished = true;
                    enabled = false;
                    return;
                }

                // var color = images[index].color;
                // color.a = 0;
                // images[index].color = color;
                images[index].enabled = true;
            }

            {
                var alpha = Mathf.PingPong(Time.time - _startTime, duration) / duration;
                var color = images[index].color;
                color.a = alpha;
                images[index].color = color;
            }
        }
    }
}