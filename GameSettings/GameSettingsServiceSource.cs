﻿namespace Game.Code.Services.GameSettingsService
{
    using Cysharp.Threading.Tasks;
    using UniGame.AddressableTools.Runtime;
    using UniGame.Core.Runtime;
    using UniGame.GameFlow.Runtime.Services;
    using UnityEngine;
    using UnityEngine.AddressableAssets;

#if UNITY_EDITOR
    using UnityEditor;
#endif

    /// <summary>
    /// game settings service
    /// </summary>
    [CreateAssetMenu(menuName = "Game/Services/Settings/GameSettings Service Source", fileName = "GameSettings Service Source")]
    public class GameSettingsServiceSource : DataSourceAsset<IGameSettingsService>
    {
        public AssetReferenceT<GameSettingsAsset> settingsAsset;

        private GameSettingsService _service;
        
        protected override async UniTask<IGameSettingsService> CreateInternalAsync(IContext context)
        {
            var lifeTime = context.LifeTime;
            var settingsData = await settingsAsset
                .LoadAssetInstanceTaskAsync(lifeTime, true);
            
            var settings = settingsData.settings;

            _service = new GameSettingsService(settings);

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
            
            return _service;
        }
        
#if UNITY_EDITOR
        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.EnteredEditMode:
                case PlayModeStateChange.ExitingEditMode:
                    EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
                    _service?.Reset();
                    break;
            }
        }
#endif
    }
}