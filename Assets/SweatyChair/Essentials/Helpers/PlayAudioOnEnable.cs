using SweatyChair.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SweatyChair
{

    public class PlayAudioOnEnable : MonoBehaviour
    {

        #region Variables

        [Header("Settings")]
        [SerializeField] private BouncyButtonAudioProvider _audioProvider;

        #endregion

        #region OnEnable

        private void OnEnable()
        {
            if (_audioProvider != null)
                _audioProvider.PlaySFX();
            else
                Debug.LogWarning($"{GetType()}: Unable to play audio clip. Provided Audio provider is null", this);
        }

        #endregion

    }

}
