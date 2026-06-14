using System;

namespace PCOptimizer.Models
{
    public class GameItem
    {
        public string Name { get; set; } = string.Empty;
        public string ExecutableName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string Launcher { get; set; } = "Kustom";
        public bool IsRunning { get; set; }
        public DateTime? LastBoosted { get; set; }
        public bool AutoBoostEnabled { get; set; } = true;
        
        // HONE-Style Details
        public string CoverImagePath { get; set; } = string.Empty; // Path to cover image poster
        public bool IsBestPerformanceApplied { get; set; } = false;
        public bool IsLiteModeApplied { get; set; } = false;
        public string WarningText => PCOptimizer.Services.TranslationService.Get("GameDetail_WarningText");
        public string GameSettingsAppliedText => PCOptimizer.Services.TranslationService.Get("GameDetail_GameSettingsAppliedText");
        public bool IsGameSettingsApplied { get; set; } = true;

        // Custom Launcher details
        public string FpsBoostStatus => IsOptimized ? PCOptimizer.Services.TranslationService.Get("Game_Boosted") : PCOptimizer.Services.TranslationService.Get("Game_Ready");
        public string PingOptimization => IsOptimized ? PCOptimizer.Services.TranslationService.Get("Emu_Active") : PCOptimizer.Services.TranslationService.Get("Emu_Inactive");
        public string PerformanceIndicator { get; set; } = "Optimal"; // Ultra, High, Optimal
        public bool IsOptimized { get; set; } = false;

        public string StatusText => IsRunning ? PCOptimizer.Services.TranslationService.Get("Game_Running") : PCOptimizer.Services.TranslationService.Get("Game_Ready");
        public string StatusColor => IsRunning ? "#00E676" : "#B0BEC5";
        public string LastBoostedText => LastBoosted.HasValue ? LastBoosted.Value.ToString("HH:mm:ss") : "-";

        public string FpsBoostLabel => PCOptimizer.Services.TranslationService.CurrentLanguage switch
        {
            "id" => "FPS Boost",
            "en" => "FPS Boost",
            "ja" => "FPSブースト",
            "zh" => "FPS提升",
            "ar" => "تعزيز FPS",
            "pt" => "Boost de FPS",
            "es" => "Boost de FPS",
            "ru" => "Ускорение FPS",
            "de" => "FPS-Boost",
            "fr" => "Boost FPS",
            "ko" => "FPS 부스트",
            "tr" => "FPS Artışı",
            _ => "FPS Boost"
        };

        public string PingTweakLabel => PCOptimizer.Services.TranslationService.CurrentLanguage switch
        {
            "id" => "Ping Tweak",
            "en" => "Ping Tweak",
            "ja" => "pingの調整",
            "zh" => "Ping优化",
            "ar" => "تحسين Ping",
            "pt" => "Ajuste de Ping",
            "es" => "Ajuste de Ping",
            "ru" => "Настройка Ping",
            "de" => "Ping-Tweak",
            "fr" => "Tweak de Ping",
            "ko" => "핑 최적화",
            "tr" => "Ping Optimizasyonu",
            _ => "Ping Tweak"
        };

        public string TargetPresetLabel => PCOptimizer.Services.TranslationService.CurrentLanguage switch
        {
            "id" => "Target Preset",
            "en" => "Target Preset",
            "ja" => "ターゲットプリセット",
            "zh" => "目标预设",
            "ar" => "الإعداد المسبق المستهدف",
            "pt" => "Predefinição Alvo",
            "es" => "Preajuste de Objetivo",
            "ru" => "Целевой пресет",
            "de" => "Ziel-Preset",
            "fr" => "Préréglage Cible",
            "ko" => "대상 프리셋",
            "tr" => "Hedef Önayarı",
            _ => "Target Preset"
        };

        public string BoostedLabel => PCOptimizer.Services.TranslationService.CurrentLanguage switch
        {
            "id" => "BOOST AKTIF",
            "en" => "BOOSTED",
            "ja" => "ブースト完了",
            "zh" => "已加速",
            "ar" => "تم التعزيز",
            "pt" => "BOOST ATIVO",
            "es" => "BOOST ATIVO",
            "ru" => "УСКОРЕНО",
            "de" => "GEBOOSTET",
            "fr" => "BOOSTÉ",
            "ko" => "부스트됨",
            "tr" => "HIZLANDIRILDI",
            _ => "BOOSTED"
        };

        public string LaunchLabel => "🚀 " + (PCOptimizer.Services.TranslationService.CurrentLanguage switch
        {
            "id" => "MULAI",
            "en" => "LAUNCH",
            "ja" => "起動",
            "zh" => "启动",
            "ar" => "تشغيل",
            "pt" => "INICIAR",
            "es" => "INICIAR",
            "ru" => "ЗАПУСК",
            "de" => "STARTEN",
            "fr" => "LANCER",
            "ko" => "실행",
            "tr" => "BAŞLAT",
            _ => "LAUNCH"
        });

        public string OptimizeLabel => IsOptimized 
            ? (PCOptimizer.Services.TranslationService.CurrentLanguage switch
            {
                "id" => "✔ SELESAI",
                "en" => "✔ READY",
                "ja" => "✔ 完了",
                "zh" => "✔ 已就绪",
                "ar" => "✔ جاهز",
                "pt" => "✔ PRONTO",
                "es" => "✔ LISTO",
                "ru" => "✔ ГОТОВО",
                "de" => "✔ BEREIT",
                "fr" => "✔ PRÊT",
                "ko" => "✔ 완료",
                "tr" => "✔ HAZIR",
                _ => "✔ READY"
            })
            : ("🛠️ " + (PCOptimizer.Services.TranslationService.CurrentLanguage switch
            {
                "id" => "OPTIMASI",
                "en" => "OPTIMIZE",
                "ja" => "最適化",
                "zh" => "优化",
                "ar" => "تحسين",
                "pt" => "OTIMIZAR",
                "es" => "OPTIMIZAR",
                "ru" => "ОПТИМИЗИРОВАТЬ",
                "de" => "OPTIMIEREN",
                "fr" => "OPTIMISER",
                "ko" => "최적화",
                "tr" => "OPTİMİZE ET",
                _ => "OPTIMIZE"
            }));
    }
}
