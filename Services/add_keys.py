import json
import os

json_path = r"c:\Users\Administrator\Documents\optimize\PCOptimizer\Services\translations.json"

with open(json_path, 'r', encoding='utf-8') as f:
    data = json.load(f)

# Define translations for new keys
new_keys_data = {
    "Msg_Success": {
        "id": "Sukses", "en": "Success", "ja": "成功", "zh": "成功", "ar": "نجاح", 
        "pt": "Sucesso", "es": "Éxito", "ru": "Успешно", "de": "Erfolgreich", 
        "fr": "Succès", "ko": "성공", "tr": "Başarılı"
    },
    "Msg_Warning": {
        "id": "Peringatan", "en": "Warning", "ja": "警告", "zh": "警告", "ar": "تحذير", 
        "pt": "Aviso", "es": "Advertencia", "ru": "Предупреждение", "de": "Warnung", 
        "fr": "Avertissement", "ko": "경고", "tr": "Uyarı"
    },
    "Msg_Error": {
        "id": "Error", "en": "Error", "ja": "エラー", "zh": "错误", "ar": "خطأ", 
        "pt": "Erro", "es": "Error", "ru": "Ошибка", "de": "Fehler", 
        "fr": "Erreur", "ko": "오류", "tr": "Hata"
    },
    "GameDetail_WarningText": {
        "id": "Game Settings dan Pro Settings memiliki kompatibilitas terbatas. Fitur ini mungkin tidak berfungsi di semua game.",
        "en": "Game Settings and Pro Settings have limited compatibility. These features may not function in all games.",
        "ja": "ゲーム設定とプロ設定は互互換性が制限されています。これらの機能はすべてのゲームで機能するとは限りません。",
        "zh": "游戏设置和专业设置的兼容性有限。这些功能可能不适用于所有游戏。",
        "ar": "إعدادات اللعبة والإعدادات الاحترافية لها توافق محدود. قد لا تعمل هذه الميزات في جميع الألعاب.",
        "pt": "As Configurações do Jogo e as Configurações Pro têm compatibilidade limitada. Esses recursos podem não funcionar em todos os jogos.",
        "es": "La configuración de juego y la configuración Pro tienen una compatibilidad limitada. Es posible que estas funciones no sirvan en todos los juegos.",
        "ru": "Настройки игры и Pro-настройки имеют ограниченную совместимость. Эти функции могут работать не во всех играх.",
        "de": "Spiel-Einstellungen und Pro-Einstellungen haben eine eingeschränkte Kompatibilität. Diese Funktionen funktionieren möglicherweise nicht in allen Spielen.",
        "fr": "Les paramètres du jeu et les paramètres Pro ont une compatibilité limitée. Ces fonctionnalités peuvent ne pas fonctionner dans tous les jeux.",
        "ko": "게임 설정 및 프로 설정은 호환성이 제한됩니다. 이러한 기능은 모든 게임에서 작동하지 않을 수 있습니다.",
        "tr": "Oyun Ayarları ve Profesyonel Ayarlar sınırlı uyumluluğa sahiptir. Bu özellikler tüm oyunlarda çalışmayabilir."
    },
    "GameDetail_GameSettingsAppliedText": {
        "id": "Performa - Direkomendasikan jika Anda ingin gameplay paling lancar. Visual game mungkin terpengaruh.",
        "en": "Performance - Recommended if you want the smoothest gameplay. Game visuals may be affected.",
        "ja": "パフォーマンス - 最もスムーズなゲームプレイが必要な場合に推奨されます。ゲームのビジュアルが影響を受ける可能性があります。",
        "zh": "性能 - 如果您想要最流畅的游戏体验，推荐使用。游戏视觉效果可能会受到影响。",
        "ar": "الأداء - يوصى به إذا كنت تريد أسلس طريقة لعب. قد تتأثر مرئيات اللعبة.",
        "pt": "Desempenho - Recomendado se você deseja a jogabilidade mais fluida. Os visuais do jogo podem ser afetados.",
        "es": "Rendimiento: recomendado si deseas un juego más fluido. Los efectos visuales del juego pueden verse afectados.",
        "ru": "Производительность — рекомендуется, если вам нужен максимально плавный игровой процесс. Это может повлиять на визуальные эффекты игры.",
        "de": "Leistung - Empfohlen, wenn Sie das flüssigste Gameplay wünschen. Die Spielgrafik kann beeinträchtigt werden.",
        "fr": "Performance - Recommandé si vous voulez le gameplay le plus fluide. Les graphismes du jeu peuvent être affectés.",
        "ko": "성능 - 가장 부드러운 게임 플레이를 원하는 경우 권장됩니다. 게임 비주얼이 영향을 받을 수 있습니다.",
        "tr": "Performans - En akıcı oynanışı istiyorsanız önerilir. Oyun görselleri etkilenebilir."
    },
    "GameMode_SelectExeTitle": {
        "id": "Pilih Executable Game Anda", "en": "Select Your Game Executable", "ja": "ゲームの実行ファイルを選択してください", "zh": "选择您的游戏可执行文件", "ar": "حدد ملف اللعبة القابل للتشغيل", "pt": "Selecione o Executável do seu Jogo", "es": "Seleccione el ejecutable de su juego", "ru": "Выберите исполняемый файл игры", "de": "Wählen Sie die ausführbare Spieldatei", "fr": "Sélectionnez l'exécutable de votre jeu", "ko": "게임 실행 파일을 선택하십시오", "tr": "Oyun Çalıştırılabilir Dosyasını Seçin"
    },
    "GameMode_GameAddedSuccess": {
        "id": "Game '{0}' berhasil ditambahkan ke daftar library!", "en": "Game '{0}' successfully added to the library list!", "ja": "ゲーム「{0}」がライブラリリストに正常に追加されました！", "zh": "游戏“{0}”已成功添加到库列表中！", "ar": "تم إضافة اللعبة '{0}' بنجاح إلى قائمة المكتبة!", "pt": "Jogo '{0}' adicionado com sucesso à lista da biblioteca!", "es": "¡El juego '{0}' se ha añadido con éxito a la lista de la biblioteca!", "ru": "Игра «{0}» успешно добавлена в список библиотеки!", "de": "Spiel '{0}' erfolgreich zur Bibliotheksliste hinzugefügt!", "fr": "Jeu '{0}' ajouté avec succès à la liste de la bibliothèque !", "ko": "게임 '{0}'이(가) 라이브러리 목록에 성공적으로 추가되었습니다!", "tr": "'{0}' oyunu kitaplık listesine başarıyla eklendi!"
    },
    "GameMode_LaunchingText": {
        "id": "Meluncurkan {0}...", "en": "Launching {0}...", "ja": "{0}を起動中...", "zh": "正在启动 {0}...", "ar": "جاري تشغيل {0}...", "pt": "Iniciando {0}...", "es": "Iniciando {0}...", "ru": "Запуск {0}...", "de": "{0} wird gestartet...", "fr": "Lancement de {0}...", "ko": "{0} 실행 중...", "tr": "{0} başlatılıyor..."
    },
    "GameMode_ExeNotFound": {
        "id": "Game executable tidak ditemukan.", "en": "Game executable not found.", "ja": "ゲームの実行ファイルが見つかりません。", "zh": "未找到游戏可执行文件。", "ar": "لم يتم العثور على ملف اللعبة القابل للتشغيل.", "pt": "Executável do jogo não encontrado.", "es": "No se encontró el ejecutable del juego.", "ru": "Исполняемый файл игры не найден.", "de": "Ausführbare Spieldatei nicht gefunden.", "fr": "Exécutable du jeu introuvable.", "ko": "게임 실행 파일을 찾을 수 없습니다.", "tr": "Oyun çalıştırılabilir dosyası bulunamadı."
    },
    "GameMode_LaunchFailed": {
        "id": "Gagal meluncurkan game: {0}", "en": "Failed to launch game: {0}", "ja": "ゲームの起動に失敗しました: {0}", "zh": "无法启动游戏: {0}", "ar": "فشل تشغيل اللعبة: {0}", "pt": "Falha ao iniciar o jogo: {0}", "es": "Error al iniciar el juego: {0}", "ru": "Не удалось запустить игру: {0}", "de": "Spiel konnte nicht gestartet werden: {0}", "fr": "Échec du lancement du jeu : {0}", "ko": "게임 실행 실패: {0}", "tr": "Oyun başlatılamadı: {0}"
    },
    "GameMode_OptAppliedText": {
        "id": "Profil optimasi diterapkan untuk {0}", "en": "Optimization profile applied for {0}", "ja": "{0}の最適化プロファイルが適用されました", "zh": "“{0}”的优化配置已应用", "ar": "تم تطبيق ملف تعريف التحسين لـ {0}", "pt": "Perfil de otimização aplicado para {0}", "es": "Perfil de optimización aplicado para {0}", "ru": "Профиль оптимизации применен для {0}", "de": "Optimierungsprofil für {0} angewendet", "fr": "Profil d'optimisation appliqué pour {0}", "ko": "{0}에 대한 최적화 프로필이 적용되었습니다.", "tr": "{0} için optimizasyon profili uygulandı"
    },
    "GameMode_OptRemovedText": {
        "id": "Profil default dikembalikan untuk {0}", "en": "Default profile restored for {0}", "ja": "{0}のデフォルトプロファイルが復元されました", "zh": "“{0}”已恢复默认配置", "ar": "تم استعادة ملف التعريف الافتراضي لـ {0}", "pt": "Perfil padrão restaurado para {0}", "es": "Perfil predeterminado restaurado para {0}", "ru": "Восстановлен профиль по умолчанию для {0}", "de": "Standardprofil für {0} wiederhergestellt", "fr": "Profil par défaut restauré pour {0}", "ko": "{0}에 대한 기본 프로필이 복원되었습니다.", "tr": "{0} için varsayılan profil geri yüklendi"
    },
    "GameDetail_RevertSuccess": {
        "id": "Pengaturan optimasi game '{0}' berhasil dikembalikan ke default Windows.", "en": "Game optimization settings for '{0}' successfully reverted to Windows defaults.", "ja": "「{0}」のゲーム最適化設定がWindowsのデフォルトに正常に戻されました。", "zh": "“{0}”的游戏优化设置已成功恢复为Windows默认值。", "ar": "تم استعادة إعدادات تحسين اللعبة لـ '{0}' بنجاح إلى افتراضيات Windows.", "pt": "As configurações de otimização de jogo para '{0}' foram revertidas com sucesso para os padrões do Windows.", "es": "La configuración de optimización del juego para '{0}' se restauró con éxito a los valores predeterminados de Windows.", "ru": "Настройки оптимизации игры для «{0}» успешно сброшены до стандартных настроек Windows.", "de": "Spieloptimierungseinstellungen für '{0}' erfolgreich auf Windows-Standardwerte zurückgesetzt.", "fr": "Paramètres d'optimisation du jeu pour '{0}' restaurés avec succès aux valeurs par défaut de Windows.", "ko": "'{0}'에 대한 게임 최적화 설정이 Windows 기본값으로 복원되었습니다.", "tr": "'{0}' için oyun optimizasyon ayarları başarıyla varsayılan Windows ayarlarına döndürüldü."
    },
    "GameDetail_RevertSuccessTitle": {
        "id": "Revert Sukses", "en": "Revert Successful", "ja": "復元完了", "zh": "恢复成功", "ar": "تمت الاستعادة بنجاح", "pt": "Reversão Concluída", "es": "Reversión Exitosa", "ru": "Сброс выполнен", "de": "Zurücksetzen erfolgreich", "fr": "Restauration réussie", "ko": "복원 완료", "tr": "Geri Yükleme Başarılı"
    },
    "GameDetail_BestPerfAppliedBody": {
        "id": "Preset 'Best Performance' berhasil diterapkan untuk '{0}'!\n\n- Dialokasikan prioritas tinggi untuk CPU & GPU.\n- 18 parameter registry game telah dioptimalkan.", "en": "Preset 'Best Performance' successfully applied for '{0}'!\n\n- High priority allocated for CPU & GPU.\n- 18 game registry parameters optimized.", "ja": "「{0}」に「最高のパフォーマンス」プリセットが適用されました！\n\n- CPUとGPUに高優先度が割り当てられました。\n- 18個のゲームレジストリパラメータが最適化されました。", "zh": "已成功为“{0}”应用“最佳性能”预设！\n\n- 为CPU和GPU分配高优先级。\n- 优化了18个游戏注册表参数。", "ar": "تم تطبيق الإعداد المسبق 'أفضل أداء' بنجاح لـ '{0}'!\n\n- تم تخصيص أولوية عالية للمعالج ووحدة معالجة الرسومات.\n- تم تحسين 18 معلمة سجل للعبة.", "pt": "Predefinição 'Melhor Desempenho' aplicada com sucesso para '{0}'!\n\n- Prioridade alta alocada para CPU e GPU.\n- 18 parâmetros de registro de jogo otimizados.", "es": "¡El preajuste 'Mejor rendimiento' se aplicó con éxito para '{0}'!\n\n- Se asignó alta prioridad para CPU y GPU.\n- Se optimizaron 18 parámetros de registro del juego.", "ru": "Пресет «Макс. производительность» успешно применен для «{0}»!\n\n- Выделен высокий приоритет для процессора и видеокарты.\n- Оптимизировано 18 параметров реестра игры.", "de": "Preset 'Beste Leistung' erfolgreich für '{0}' angewendet!\n\n- Hohe Priorität für CPU & GPU zugewiesen.\n- 18 Spielregistrierungsparameter optimiert.", "fr": "Préréglage 'Meilleures performances' appliqué avec succès pour '{0}' !\n\n- Priorité élevée allouée pour CPU et GPU.\n- 18 paramètres de registre du jeu optimisés.", "ko": "'{0}'에 대한 '최고 성능' 프리셋이 적용되었습니다!\n\n- CPU 및 GPU에 높은 우선순위가 할당되었습니다.\n- 18개의 게임 레지스트리 매개변수가 최적화되었습니다.", "tr": "'{0}' için 'En İyi Performans' önayarı başarıyla uygulandı!\n\n- CPU ve GPU için yüksek öncelik ayrıldı.\n- 18 oyun kayıt defteri parametresi optimize edildi."
    },
    "GameDetail_LiteModeAppliedBody": {
        "id": "Preset 'Lite Mode' berhasil diterapkan untuk '{0}'!\n\n- RAM Standby dilepas saat game dinyalakan.\n- 8 parameter optimal sistem diterapkan.", "en": "Preset 'Lite Mode' successfully applied for '{0}'!\n\n- Standby RAM released when launching game.\n- 8 optimal system parameters applied.", "ja": "「{0}」に「ライトモード」プリセットが適用されました！\n\n- ゲーム起動時にスタンバイRAMが解放されます。\n- 8個の最適なシステムパラメータが適用されました。", "zh": "已成功为“{0}”应用“极速模式”预设！\n\n- 启动游戏时释放待机内存。\n- 应用了8个最佳系统参数。", "ar": "تم تطبيق الإعداد المسبق 'الوضع الخفيف' بنجاح لـ '{0}'!\n\n- تم تحرير ذاكرة الوصول العشوائي في وضع الاستعداد عند تشغيل اللعبة.\n- تم تطبيق 8 معلمات نظام مثالية.", "pt": "Predefinição 'Modo Lite' aplicada com sucesso para '{0}'!\n\n- RAM em standby liberada ao iniciar o jogo.\n- 8 parâmetros de sistema ideais aplicados.", "es": "¡El preajuste 'Modo Lite' se aplicó con éxito para '{0}'!\n\n- Se liberó la RAM en espera al iniciar el juego.\n- Se aplicaron 8 parámetros óptimos del sistema.", "ru": "Пресет «Легкий режим» успешно применен для «{0}»!\n\n- Освобождена оперативная память в режиме ожидания при запуске игры.\n- Применено 8 оптимальных параметров системы.", "de": "Preset 'Lite-Modus' erfolgreich für '{0}' angewendet!\n\n- Standby-RAM beim Starten des Spiels freigegeben.\n- 8 optimale Systemparameter angewendet.", "fr": "Préréglage 'Mode Lite' appliqué avec succès pour '{0}' !\n\n- RAM de veille libérée au lancement du jeu.\n- 8 paramètres système optimaux appliqués.", "ko": "'{0}'에 대한 '라이트 모드' 프리셋이 적용되었습니다!\n\n- 게임 실행 시 대기 RAM이 해제됩니다.\n- 8개의 최적의 시스템 매개변수가 적용되었습니다.", "tr": "'{0}' için 'Hafif Mod' önayarı başarıyla uygulandı!\n\n- Oyun başlatılırken beklemedeki RAM serbest bırakıldı.\n- 8 optimal sistem parametresi uygulandı."
    },
    "GameDetail_OptSuccessTitle": {
        "id": "Optimasi Game Sukses", "en": "Game Optimization Successful", "ja": "ゲームの最適化成功", "zh": "游戏优化成功", "ar": "نجح تحسين اللعبة", "pt": "Otimização de Jogo Concluída", "es": "Optimización del juego exitosa", "ru": "Оптимизация игры выполнена", "de": "Spieloptimierung erfolgreich", "fr": "Optimisation du jeu réussie", "ko": "게임 최적화 성공", "tr": "Oyun Optimizasyonu Başarılı"
    },
    "GameDetail_OptFailed": {
        "id": "Gagal menerapkan optimasi game: {0}", "en": "Failed to apply game optimization: {0}", "ja": "ゲームの最適化の適用に失敗しました: {0}", "zh": "无法应用游戏优化: {0}", "ar": "فشل تطبيق تحسين اللعبة: {0}", "pt": "Falha ao aplicar otimização do jogo: {0}", "es": "Error al aplicar la optimización del juego: {0}", "ru": "Не удалось применить оптимизацию игры: {0}", "de": "Spieloptimierung konnte nicht angewendet werden: {0}", "fr": "Échec de l'application de l'optimisation du jeu : {0}", "ko": "게임 최적화 적용 실패: {0}", "tr": "Oyun optimizasyonu uygulanamadı: {0}"
    },
    "Mouse_Msg_AimEnabled": {
        "id": "MarkC Mouse Fix sukses diterapkan! Mouse acceleration dimatikan (Aim 1:1 aktif).", "en": "MarkC Mouse Fix successfully applied! Mouse acceleration disabled (1:1 Aim active).", "ja": "MarkCマウスフィックスが正常に適用されました！マウスアクセラレーションが無効になりました（1:1エイム有効）。", "zh": "MarkC鼠标修正已成功应用！鼠标加速已禁用（1:1瞄准已激活）。", "ar": "تم تطبيق إصلاح الماوس MarkC بنجاح! تم تعطيل تسريع الماوس (هدف 1:1 نشط).", "pt": "MarkC Mouse Fix aplicado com sucesso! Aceleração do mouse desativada (Aim 1:1 ativo).", "es": "¡MarkC Mouse Fix se aplicó con éxito! Aceleración del mouse desactivada (Aim 1:1 activo).", "ru": "MarkC Mouse Fix успешно применен! Акселерация мыши отключена (активен прицел 1:1).", "de": "MarkC Mouse Fix erfolgreich angewendet! Mausbeschleunigung deaktiviert (1:1 Aim aktiv).", "fr": "MarkC Mouse Fix appliqué avec succès ! Accélération de la souris désactivée (Aim 1:1 actif).", "ko": "MarkC 마우스 수정이 성공적으로 적용되었습니다! 마우스 가속이 비활성화되었습니다 (1:1 조준 활성화).", "tr": "MarkC Fare Düzeltmesi başarıyla uygulandı! Fare hızlandırması devre dışı bırakıldı (1:1 Nişan aktif)."
    },
    "Mouse_Msg_AimDisabled": {
        "id": "Pengaturan mouse dikembalikan ke default Windows (Akselerasi aktif).", "en": "Mouse settings restored to Windows defaults (Acceleration active).", "ja": "マウス設定がWindowsのデフォルトに復元されました（アクセラレーション有効）。", "zh": "鼠标设置已恢复为Windows默认值（加速已启用）。", "ar": "تم استعادة إعدادات الماوس إلى افتراضيات Windows (التسريع نشط).", "pt": "Configurações do mouse restauradas para os padrões do Windows (Aceleração ativa).", "es": "Configuración del mouse restaurada a los valores predeterminados de Windows (Aceleración activa).", "ru": "Настройки мыши восстановлены до стандартных настроек Windows (акселерация включена).", "de": "Mauseinstellungen auf Windows-Standardwerte zurückgesetzt (Beschleunigung aktiv).", "fr": "Paramètres de la souris restaurés aux valeurs par défaut de Windows (Accélération active).", "ko": "마우스 설정이 Windows 기본값으로 복원되었습니다 (가속 활성화).", "tr": "Fare ayarları varsayılan Windows ayarlarına döndürüldü (Hızlandırma aktif)."
    },
    "Mouse_Msg_KbEnabled": {
        "id": "Optimasi Keyboard sukses! Delay input diminimalkan.", "en": "Keyboard optimization successful! Input delay minimized.", "ja": "キーボードの最適化が成功しました！入力遅延が最小限に抑えられました。", "zh": "键盘优化成功！输入延迟降至最低。", "ar": "نجح تحسين لوحة المفاتيح! تم تقليل تأخير الإدخال.", "pt": "Otimização do teclado bem-sucedida! Atraso de entrada minimizado.", "es": "¡Optimización del teclado exitosa! Retardo de entrada minimizado.", "ru": "Оптимизация клавиатуры выполнена успешно! Задержка ввода сведена к минимуму.", "de": "Tastaturoptimierung erfolgreich! Eingabeverzögerung minimiert.", "fr": "Optimisation du clavier réussie ! Délai d'entrée minimisé.", "ko": "키보드 최적화 성공! 입력 지연이 최소화되었습니다.", "tr": "Klavye optimizasyonu başarılı! Giriş gecikmesi en aza indirildi."
    },
    "Mouse_Msg_KbDisabled": {
        "id": "Keyboard dikembalikan ke setting delay default Windows.", "en": "Keyboard restored to default Windows delay settings.", "ja": "キーボードがWindowsのデフォルトの遅延設定に復元されました。", "zh": "键盘已恢复为默认的Windows延迟设置。", "ar": "تم استعادة لوحة المفاتيح إلى إعدادات التأخير الافتراضية لـ Windows.", "pt": "Teclado restaurado para as configurações de atraso padrão do Windows.", "es": "El teclado se restauró a la configuración de retardo predeterminada de Windows.", "ru": "Клавиатура восстановлена до стандартных настроек задержки Windows.", "de": "Tastatur auf Standard-Windows-Verzögerungseinstellungen zurückgesetzt.", "fr": "Clavier restauré aux paramètres de délai par défaut de Windows.", "ko": "키보드가 Windows 기본 지연 설정으로 복원되었습니다.", "tr": "Klavye varsayılan Windows gecikme ayarlarına döndürüldü."
    },
    "Mouse_Msg_UsbEnabled": {
        "id": "USB Selective Suspend dinonaktifkan! Periferal tidak akan hemat daya secara mendadak.", "en": "USB Selective Suspend disabled! Peripherals won't randomly enter power-saving mode.", "ja": "USBセレクティブサスペンドが無効になりました！周辺機器がランダムに省電力モードに入ることはありません。", "zh": "USB选择性 suspended 已禁用！外设不会随机进入省电模式。", "ar": "تم تعطيل تعليق USB الانتقائي! لن تدخل الأجهزة الطرفية بشكل عشوائي في وضع توفير الطاقة.", "pt": "Suspensão seletiva USB desativada! Os periféricos não entrarão aleatoriamente no modo de economia de energia.", "es": "¡Suspensión selectiva de USB desactivada! Los periféricos no entrarán en modo de ahorro de energía al azar.", "ru": "Выборочная приостановка USB отключена! Периферийные устройства не будут случайно переходить в режим энергосбережения.", "de": "Selektives USB-Energiesparen deaktiviert! Peripheriegeräte wechseln nicht zufällig in den Energiesparmodus.", "fr": "Suspension sélective USB désactivée ! Les périphériques n'entreront pas de manière aléatoire en mode d'économie d'énergie.", "ko": "USB 선택적 절전 모드가 비활성화되었습니다! 주변 장치가 임의로 절전 모드로 들어가지 않습니다.", "tr": "USB Seçici Askıya Alma devre dışı bırakıldı! Çevre birimleri rastgele güç tasarrufu moduna girmeyecektir."
    },
    "Mouse_Msg_UsbDisabled": {
        "id": "USB Selective Suspend dikembalikan ke pengaturan default Windows.", "en": "USB Selective Suspend restored to Windows defaults.", "ja": "USBセレクティブサスペンドがWindowsのデフォルトに復元されました。", "zh": "USB选择性 suspended 已恢复为Windows默认值。", "ar": "تم استعادة تعليق USB الانتقائي إلى افتراضيات Windows.", "pt": "Suspensão seletiva USB restaurada para os padrões do Windows.", "es": "La suspensión selectiva de USB se restauró a los valores predeterminados de Windows.", "ru": "Выборочная приостановка USB восстановлена до стандартных настроек Windows.", "de": "Selektives USB-Energiesparen auf Windows-Standardwerte zurückgesetzt.", "fr": "Suspension sélective USB restaurée aux valeurs par défaut de Windows.", "ko": "USB 선택적 절전 모드가 Windows 기본값으로 복원되었습니다.", "tr": "USB Seçici Askıya Alma varsayılan Windows ayarlarına döndürüldü."
    },
    "Mouse_Msg_PollEnabled": {
        "id": "Prioritas Polling USB ditingkatkan pada CPU untuk mouse 1000Hz+.", "en": "USB Polling Priority increased on CPU for 1000Hz+ mice.", "ja": "1000Hz以上のマウスのCPUでUSBポーリング優先度が向上しました。", "zh": "针对 1000Hz+ 鼠标提高了 CPU 上的 USB 轮询优先级。", "ar": "تم زيادة أولوية استطلاع USB على المعالج لماوس 1000Hz+.", "pt": "Prioridade de polling USB aumentada na CPU para mouses de 1000Hz+.", "es": "Prioridad de sondeo USB aumentada en la CPU para ratones de 1000Hz+.", "ru": "Приоритет опроса USB повышен в процессоре для мышей с частотой 1000 Гц и более.", "de": "USB-Polling-Priorität auf der CPU für 1000Hz+-Mäuse erhöht.", "fr": "Priorité d'interrogation USB augmentée sur le processeur pour les souris 1000Hz+.", "ko": "1000Hz+ 마우스의 CPU에서 USB 폴링 우선 순위가 향상되었습니다.", "tr": "1000Hz+ fareler için CPU'da USB Yoklama Önceliği artırıldı."
    },
    "Mouse_Msg_PollDisabled": {
        "id": "Prioritas Polling USB dikembalikan ke default Windows.", "en": "USB Polling Priority restored to Windows defaults.", "ja": "USBポーリング優先度がWindowsのデフォルトに復元されました。", "zh": "USB 轮询优先级已恢复为Windows默认值。", "ar": "تم استعادة أولوية استطلاع USB إلى افتراضيات Windows.", "pt": "Prioridade de polling USB restaurada para os padrões do Windows.", "es": "La prioridad de sondeo USB se restauró a los valores predeterminados de Windows.", "ru": "Приоритет опроса USB восстановлен до стандартных настроек Windows.", "de": "USB-Polling-Priorität auf Windows-Standardwerte zurückgesetzt.", "fr": "Priorité d'interrogation USB restaurée aux valeurs par défaut de Windows.", "ko": "USB 폴링 우선 순위가 Windows 기본값으로 복원되었습니다.", "tr": "USB Yoklama Önceliği varsayılan Windows ayarlarına döndürüldü."
    },
    "Mouse_Msg_AdminRequired": {
        "id": "Gagal mengakses PriorityControl registry. Memerlukan hak Administrator.", "en": "Failed to access PriorityControl registry. Administrator rights required.", "ja": "PriorityControlレジストリへのアクセスに失敗しました。管理者権限が必要です。", "zh": "无法访问 PriorityControl 注册表。需要管理员权限。", "ar": "فشل الوصول إلى سجل PriorityControl. يتطلب صلاحيات المسؤول.", "pt": "Falha ao acessar o registro PriorityControl. Direitos de administrador necessários.", "es": "Error al acceder al registro PriorityControl. Se requieren derechos de administrador.", "ru": "Не удалось получить доступ к реестру PriorityControl. Требуются права администратора.", "de": "Zugriff auf die PriorityControl-Registrierung fehlgeschlagen. Administratorrechte erforderlich.", "fr": "Échec de l'accès au registre PriorityControl. Droits d'administrateur requis.", "ko": "PriorityControl 레지스트리에 액세스하지 못했습니다. 관리자 권한이 필요합니다.", "tr": "PriorityControl kayıt defterine erişilemedi. Yönetici hakları gereklidir."
    },
    "Mouse_Msg_SensiApplied": {
        "id": "Pengaturan sensitivitas berhasil diterapkan!\nKecepatan Pointer: {0}\nSens X: {1:F2}x | Sens Y: {2:F2}x\n(Kompatibel untuk VALORANT & Minecraft)",
        "en": "Sensitivity settings successfully applied!\nPointer Speed: {0}\nSens X: {1:F2}x | Sens Y: {2:F2}x\n(Compatible with VALORANT & Minecraft)",
        "ja": "感度設定が正常に適用されました！\nポインタ速度: {0}\nSens X: {1:F2}x | Sens Y: {2:F2}x\n（VALORANTおよびMinecraftと互換性あり）",
        "zh": "灵敏度设置已成功应用！\n指针速度: {0}\n水平灵敏度: {1:F2}x | 垂直灵敏度: {2:F2}x\n（兼容瓦罗兰特与我的世界）",
        "ar": "تم تطبيق إعدادات الحساسية بنجاح!\nسرعة المؤشر: {0}\nحساسية X: {1:F2}x | حساسية Y: {2:F2}x\n(متوافق مع VALORANT و Minecraft)",
        "pt": "Configurações de sensibilidade aplicadas com sucesso!\nVelocidade do ponteiro: {0}\nSens X: {1:F2}x | Sens Y: {2:F2}x\n(Compatível com VALORANT e Minecraft)",
        "es": "¡Configuración de sensibilidad aplicada con éxito!\nVelocidad del puntero: {0}\nSens X: {1:F2}x | Sens Y: {2:F2}x\n(Compatible con VALORANT y Minecraft)",
        "ru": "Настройки чувствительности успешно применены!\nСкорость указателя: {0}\nЧувств. X: {1:F2}x | Чувств. Y: {2:F2}x\n(Совместимо с VALORANT и Minecraft)",
        "de": "Empfindlichkeitseinstellungen erfolgreich angewendet!\nZeigergeschwindigkeit: {0}\nSens X: {1:F2}x | Sens Y: {2:F2}x\n(Kompatibel mit VALORANT & Minecraft)",
        "fr": "Paramètres de sensibilité appliqués avec succès !\nVitesse du pointeur : {0}\nSens X : {1:F2}x | Sens Y : {2:F2}x\n(Compatible avec VALORANT & Minecraft)",
        "ko": "감도 설정이 성공적으로 적용되었습니다!\n포인터 속도: {0}\n감도 X: {1:F2}x | 감도 Y: {2:F2}x\n(발로란트 및 마인크래프트와 호환)",
        "tr": "Hassasiyet ayarları başarıyla uygulandı!\nİşaretçi Hızı: {0}\nHass X: {1:F2}x | Hass Y: {2:F2}x\n(VALORANT ve Minecraft ile uyumlu)"
    },
    "Mouse_Msg_SensiFailed": {
        "id": "Gagal menerapkan sensitivitas mouse: {0}", "en": "Failed to apply mouse sensitivity: {0}", "ja": "マウスの感度の適用に失敗しました: {0}", "zh": "无法应用鼠标灵敏度: {0}", "ar": "فشل تطبيق حساسية الماوس: {0}", "pt": "Falha ao aplicar sensibilidade do mouse: {0}", "es": "Error al aplicar la sensibilidad del mouse: {0}", "ru": "Не удалось применить чувствительность мыши: {0}", "de": "Mausempfindlichkeit konnte nicht angewendet werden: {0}", "fr": "Échec de l'application de la sensibilité de la souris : {0}", "ko": "마우스 감도 적용 실패: {0}", "tr": "Fare hassasiyeti uygulanamadı: {0}"
    },
    "Mouse_Msg_HwSaved": {
        "id": "Konfigurasi hardware berhasil disimpan!\nMouse diselaraskan pada {0} DPI dan {1} Hz Polling Rate.",
        "en": "Hardware configuration successfully saved!\nMouse aligned to {0} DPI and {1} Hz Polling Rate.",
        "ja": "ハードウェア構成が正常に保存されました！\nマウスは {0} DPI と {1} Hz のポーリングレートに同期されました。",
        "zh": "硬件配置已成功保存！\n鼠标已对齐至 {0} DPI 和 {1} Hz 轮询率。",
        "ar": "تم حفظ تكوين الأجهزة بنجاح!\nتم محاذاة الماوس عند {0} DPI ومعدل استطلاع {1} هرتز.",
        "pt": "Configuração de hardware salva com sucesso!\nMouse alinhado para {0} DPI e {1} Hz Polling Rate.",
        "es": "¡Configuración de hardware guardada con éxito!\nMouse alineado a {0} DPI y {1} Hz de tasa de sondeo.",
        "ru": "Аппаратная конфигурация успешно сохранена!\nМышь настроена на {0} DPI и частоту опроса {1} Гц.",
        "de": "Hardwarekonfiguration erfolgreich gespeichert!\nMaus auf {0} DPI und {1} Hz Polling-Rate ausgerichtet.",
        "fr": "Configuration matérielle enregistrée avec succès !\nSouris alignée sur {0} DPI et un taux d'interrogation de {1} Hz.",
        "ko": "하드웨어 구성이 성공적으로 저장되었습니다!\n마우스가 {0} DPI 및 {1} Hz 폴링 레이트로 동기화되었습니다.",
        "tr": "Donanım yapılandırması başarıyla kaydedildi!\nFare {0} DPI ve {1} Hz Yoklama Hızına hizalandı."
    },
    "Mouse_Msg_HwFailed": {
        "id": "Gagal menyimpan konfigurasi hardware: {0}", "en": "Failed to save hardware configuration: {0}", "ja": "ハードウェア構成の保存に失敗しました: {0}", "zh": "无法保存硬件配置: {0}", "ar": "فشل حفظ تكوين الأجهزة: {0}", "pt": "Falha ao salvar configuração de hardware: {0}", "es": "Error al guardar la configuración de hardware: {0}", "ru": "Не удалось сохранить аппаратную конфигурацию: {0}", "de": "Hardwarekonfiguration konnte nicht gespeichert werden: {0}", "fr": "Échec de l'enregistrement de la configuration matérielle : {0}", "ko": "하드웨어 구성 저장 실패: {0}", "tr": "Donanım yapılandırması kaydedilemedi: {0}"
    },
    "Mouse_Msg_ExportSuccess": {
        "id": "Konfigurasi berhasil diekspor!", "en": "Configuration successfully exported!", "ja": "構成が正常にエクスポートされました！", "zh": "配置已成功导出！", "ar": "تم تصدير التكوين بنجاح!", "pt": "Configuração exportada com sucesso!", "es": "¡Configuración exportada con éxito!", "ru": "Конфигурация успешно экспортирована!", "de": "Konfiguration erfolgreich exportiert!", "fr": "Configuration exportée avec succès !", "ko": "구성 내보내기 성공!", "tr": "Yapılandırma başarıyla dışa aktarıldı!"
    },
    "Mouse_Msg_ExportFailed": {
        "id": "Gagal mengekspor konfigurasi: {0}", "en": "Failed to export configuration: {0}", "ja": "構成のエクスポートに失敗しました: {0}", "zh": "无法导出配置: {0}", "ar": "فشل تصدير التكوين: {0}", "pt": "Falha ao exportar configuração: {0}", "es": "Error al exportar la configuración: {0}", "ru": "Не удалось экспортировать конфигурацию: {0}", "de": "Konfiguration konnte nicht exportiert werden: {0}", "fr": "Échec de l'exportation de la configuration : {0}", "ko": "구성 내보내기 실패: {0}", "tr": "Yapılandırma dışa aktarılamadı: {0}"
    },
    "Mouse_Msg_ImportSuccess": {
        "id": "Konfigurasi berhasil diimpor dan diterapkan!", "en": "Configuration successfully imported and applied!", "ja": "構成が正常にインポートされて適用されました！", "zh": "配置已成功导入并应用！", "ar": "تم استيراد التكوين وتطبيقه بنجاح!", "pt": "Configuração importada e aplicada com sucesso!", "es": "¡Configuración importada y aplicada con éxito!", "ru": "Конфигурация успешно импортирована и применена!", "de": "Konfiguration erfolgreich importiert und angewendet!", "fr": "Configuration importée et appliquée avec succès !", "ko": "구성을 성공적으로 가져오고 적용했습니다!", "tr": "Yapılandırma başarıyla içe aktarıldı ve uygulandı!"
    },
    "Mouse_Msg_ImportFailed": {
        "id": "Gagal mengimpor konfigurasi: {0}", "en": "Failed to import configuration: {0}", "ja": "構成のインポートに失敗しました: {0}", "zh": "无法导入配置: {0}", "ar": "فشل استيراد التكوين: {0}", "pt": "Falha ao importar configuração: {0}", "es": "Error al importar la configuración: {0}", "ru": "Не удалось импортировать конфигурацию: {0}", "de": "Konfiguration konnte nicht importiert werden: {0}", "fr": "Échec de l'importation de la configuration : {0}", "ko": "구성 가져오기 실패: {0}", "tr": "Yapılandırma içe aktarılamadı: {0}"
    },
    "Startup_Msg_LoadFailed": {
        "id": "Gagal memuat daftar startup: {0}", "en": "Failed to load startup list: {0}", "ja": "スタートアップリストの読み込みに失敗しました: {0}", "zh": "无法加载启动列表: {0}", "ar": "فشل تحميل قائمة بدء التشغيل: {0}", "pt": "Falha ao carregar a lista de inicialização: {0}", "es": "Error al cargar la lista de inicio: {0}", "ru": "Не удалось загрузить список автозагрузки: {0}", "de": "Startliste konnte nicht geladen werden: {0}", "fr": "Échec du chargement de la liste de démarrage : {0}", "ko": "시작 프로그램 목록 로드 실패: {0}", "tr": "Başlangıç listesi yüklenemedi: {0}"
    },
    "Startup_Msg_EnableFailed": {
        "id": "Gagal mengaktifkan item startup. Memerlukan hak Administrator.", "en": "Failed to enable startup item. Administrator rights required.", "ja": "スタートアップ項目の有効化に失敗しました。管理者権限が必要です。", "zh": "无法启用启动项。需要管理员权限。", "ar": "فشل تمكين عنصر بدء التشغيل. يتطلب صلاحيات المسؤول.", "pt": "Falha ao habilitar o item de inicialização. Direitos de administrador necessários.", "es": "Error al habilitar el elemento de inicio. Se requieren derechos de administrador.", "ru": "Не удалось включить элемент автозагрузки. Требуются права администратора.", "de": "Startseitelement konnte nicht aktiviert werden. Administratorrechte erforderlich.", "fr": "Échec de l'activation de l'élément de démarrage. Droits d'administrateur requis.", "ko": "시작 프로그램을 활성화하지 못했습니다. 관리자 권한이 필요합니다.", "tr": "Başlangıç öğesi etkinleştirilemedi. Yönetici hakları gereklidir."
    },
    "Startup_Msg_DisableFailed": {
        "id": "Gagal menonaktifkan item startup. Memerlukan hak Administrator.", "en": "Failed to disable startup item. Administrator rights required.", "ja": "スタートアップ項目の無効化に失敗しました。管理者権限が必要です。", "zh": "无法禁用启动项。需要管理员权限。", "ar": "فشل تعطيل عنصر بدء التشغيل. يتطلب صلاحيات المسؤول.", "pt": "Falha ao desabilitar o item de inicialização. Direitos de administrador necessários.", "es": "Error al deshabilitar el elemento de inicio. Se requieren derechos de administrador.", "ru": "Не удалось отключить элемент автозагрузки. Требуются права администратора.", "de": "Startseitelement konnte nicht deaktiviert werden. Administratorrechte erforderlich.", "fr": "Échec de la désactivation de l'élément de démarrage. Droits d'administrateur requis.", "ko": "시작 프로그램을 비활성화하지 못했습니다. 관리자 권한이 필요합니다.", "tr": "Başlangıç öğesi devre dışı bırakılamadı. Yönetici hakları gereklidir."
    },
    "Emu_Desc_BlueStacks": {
        "id": "Meningkatkan prioritas thread CPU dan mengunci proses pada physical cores untuk menghilangkan micro-stuttering.",
        "en": "Increases CPU thread priority and locks processes to physical cores to eliminate micro-stuttering.",
        "ja": "CPUスレッドの優先度を上げ、プロセスを物理コアにロックしてマイクロスタッターを排除します。",
        "zh": "提高CPU线程优先级并将进程锁定到物理核心，以消除微卡顿。",
        "ar": "يزيد من أولوية خيط المعالج ويقفل العمليات على النوى المادية للقضاء على التقطيع الدقيق.",
        "pt": "Aumenta a prioridade do thread da CPU e bloqueia processos nos núcleos físicos para eliminar micro-stuttering.",
        "es": "Aumenta la prioridad del hilo de la CPU y bloquea los procesos en los núcleos físicos para eliminar los micro tirones.",
        "ru": "Повышает приоритет потоков процессора и привязывает процессы к физическим ядрам для устранения микрофризов.",
        "de": "Erhöht die CPU-Thread-Priorität und sperrt Prozesse auf physische Kerne, um Mikroruckler zu vermeiden.",
        "fr": "Augmente la priorité des threads CPU et verrouille les processus sur les cœurs physiques para éliminer les micro-saccades.",
        "ko": "CPU 스레드 우선 순위를 높이고 프로세스를 물리적 코어에 잠가 미세한 끊김을 제거합니다.",
        "tr": "Mikro kekelemeyi ortadan kaldırmak için CPU iş parçacığı önceliğini artırır ve işlemleri fiziksel çekirdeklere kilitler."
    },
    "Emu_Desc_LDPlayer": {
        "id": "Membatasi logical hyperthreads dan memaksa render GPU berdaya tinggi untuk stabilitas FPS.",
        "en": "Limits logical hyperthreads and forces high-power GPU rendering for FPS stability.",
        "ja": "論理ハイパースレッドを制限し、FPS安定性のために高出力GPUレンダリングを強制します。",
        "zh": "限制逻辑超线程并强制高功率GPU渲染，以实现FPS稳定。",
        "ar": "يحد من خيوط المعالجة المتعددة الفائقة ويفرض عرض وحدة معالجة الرسومات عالية الطاقة لاستقرار FPS.",
        "pt": "Limita hyperthreads lógicos e força renderização de GPU de alta potência para estabilidade de FPS.",
        "es": "Limita los hiperprocesos lógicos y fuerza el renderizado de GPU de alta potencia para la estabilidad de FPS.",
        "ru": "Ограничивает логические потоки гипертрединга и принудительно включает высокопроизводительный рендеринг на GPU для стабильности FPS.",
        "de": "Begrenzt logische Hyperthreads und erzwingt High-Power-GPU-Rendering für FPS-Stabilität.",
        "fr": "Limite les hyperthreads logiques et force le rendu GPU haute puissance pour la stabilité du FPS.",
        "ko": "논리적 하이퍼스레드를 제한하고 FPS 안정성을 위해 고전력 GPU 렌더링을 강제합니다.",
        "tr": "FPS kararlılığı için mantıksal hiper iş parçacıklarını sınırlar ve yüksek güçlü GPU işlemeyi zorlar."
    },
    "Emu_Desc_NoxPlayer": {
        "id": "Meningkatkan alokasi RAM proses emulator dan mengurangi latency input mouse/keyboard.",
        "en": "Increases emulator process RAM allocation and reduces mouse/keyboard input latency.",
        "ja": "エミュレータプロセスのRAM割り当てを増やし、マウス/キーボードの入力遅延を減らします。",
        "zh": "增加模拟器进程的内存分配，并降低鼠标/键盘输入延迟。",
        "ar": "يزيد من تخصيص ذاكرة الوصول العشوائي لعملية المحاكي ويقلل من زمن انتقال إدخال الماوس/لوحة المفاتيح.",
        "pt": "Aumenta a alocação de RAM do processo do emulador e reduz a latência de entrada do mouse/teclado.",
        "es": "Aumenta la asignación de RAM del processo del emulador y reduce la latencia de entrada del mouse y teclado.",
        "ru": "Увеличивает объем оперативной памяти для процесса эмулятора и снижает задержку ввода с мыши и клавиатуры.",
        "de": "Erhöht die RAM-Zuweisung des Emulatorprozesses und reduziert die Eingabelatenz von Maus und Tastatur.",
        "fr": "Augmente l'allocation RAM du processus de l'émulateur et réduit la latence d'entrée de la souris et du clavier.",
        "ko": "에뮬레이터 프로세스 RAM 할당을 늘리고 마우스/키보드 입력 지연 시간을 줄입니다.",
        "tr": "Emülatör işlemi RAM tahsisini artırır ve fare/klavye giriş gecikmesini azaltır."
    },
    "Emu_Desc_MEmu": {
        "id": "Mengoptimasi alokasi core CPU dan sinkronisasi driver grafis agar loading game lebih responsif.",
        "en": "Optimizes CPU core allocation and graphics driver synchronization for more responsive game loading.",
        "ja": "ゲームのロードをより応答性にするために、CPUコアの割り当てとグラフィックスドライバーの同期を最適化します。",
        "zh": "优化CPU核心分配和图形驱动程序同步，使游戏加载更具响应性。",
        "ar": "يصلح تخصيص نواة المعالج ومزامنة برنامج تشغيل الرسومات لجعل تحميل اللعبة أكثر استجابة.",
        "pt": "Otimiza a alocação de núcleos de CPU e a sincronização do driver gráfico para um carregamento de jogo mais responsivo.",
        "es": "Optimiza la asignación de núcleos de CPU y la sincronización del controlador de gráficos para un inicio de juego más rápido.",
        "ru": "Оптимизирует распределение ядер процессора и синхронизацию графического драйвера для более быстрого запуска игр.",
        "de": "Optimiert die CPU-Kernzuweisung und die Grafiktreibersynchronisation für ein schnelleres Laden von Spielen.",
        "fr": "Optimise l'allocation des cœurs CPU et la synchronisation du pilote graphique pour un chargement de jeu plus réactif.",
        "ko": "더 반응이 빠른 게임 로딩을 위해 CPU 코어 할당 및 그래픽 드라이버 동기화를 최적화합니다.",
        "tr": "Oyun yüklemenin daha duyarlı olması için CPU çekirdeği tahsisini ve grafik sürücüsü senkronizasyonunu optimize eder."
    },
    "Emu_Desc_GameLoop": {
        "id": "Mengoptimasi prioritas mesin virtual Tencent (TGB) untuk stabilitas FPS di PUBG Mobile/CODM.",
        "en": "Optimizes Tencent Virtual Machine (TGB) priority for FPS stability in PUBG Mobile/CODM.",
        "ja": "PUBG Mobile/CODMでのFPS安定性のために、Tencent仮想マシン（TGB）の優先度を最適化します。",
        "zh": "优化腾讯虚拟机（TGB）优先级，以提高和平精英/CODM等游戏中的FPS稳定性。",
        "ar": "يُحسن أولوية جهاز Tencent الافتراضي (TGB) لاستقرار FPS في PUBG Mobile/CODM.",
        "pt": "Otimiza a prioridade da Máquina Virtual Tencent (TGB) para estabilidade de FPS no PUBG Mobile/CODM.",
        "es": "Optimiza la prioridad de la máquina virtual Tencent (TGB) para la estabilidad de FPS en PUBG Mobile/CODM.",
        "ru": "Оптимизирует приоритет виртуальной машины Tencent (TGB) для стабильности FPS в PUBG Mobile/CODM.",
        "de": "Optimiert die Priorität der Tencent Virtual Machine (TGB) für die FPS-Stabilität in PUBG Mobile/CODM.",
        "fr": "Optimise la priorité de la machine virtuelle Tencent (TGB) pour la stabilité du FPS dans PUBG Mobile/CODM.",
        "ko": "PUBG Mobile/CODM의 FPS 안정성을 위해 Tencent 가상 머신(TGB) 우선 순위를 최적화합니다.",
        "tr": "PUBG Mobile/CODM'de FPS kararlılığı için Tencent Sanal Makinesi (TGB) önceliğini optimize eder."
    },
    "Emu_Desc_MSI": {
        "id": "Mengunci core fisik proses player dan mengaktifkan bandwidth disk penuh untuk optimalisasi loading.",
        "en": "Locks physical cores of player process and enables full disk bandwidth for loading optimization.",
        "ja": "プロセスの物理コアをロックし、ロードの最適化のためにフルディスク帯域幅を有効にします。",
        "zh": "锁定播放器进程的物理核心，并启用全磁盘带宽以进行加载优化。",
        "ar": "يقفل النوى المادية لعملية المشغل ويمكن عرض نطاق القرص الكامل لتحسين التحميل.",
        "pt": "Bloqueia núcleos físicos do processo do player e habilita largura de banda total do disco para otimização de carregamento.",
        "es": "Bloquea los núcleos físicos del proceso del reproductor y habilita el ancho de banda del disco completo para la optimización de la carga.",
        "ru": "Блокирует физические ядра процесса плеера и включает максимальную пропускную способность диска для оптимизации загрузки.",
        "de": "Sperrt die physischen Kerne des Playerprozesses und aktiviert die volle Festplattenbandbreite für die Ladeoptimierung.",
        "fr": "Verrouille les cœurs physiques du processus du lecteur et active la bande passante totale du disque pour l'optimisation du chargement.",
        "ko": "플레이어 프로세스의 물리적 코어를 잠그고 로딩 최적화를 위해 디스크 대역폭을 완전히 활성화합니다.",
        "tr": "Oynatıcı işleminin fiziksel çekirdeklerini kilitler ve yükleme optimizasyonu için tam disk bant genişliğini etkinleştirir."
    },
    "Emu_Desc_MuMu": {
        "id": "Mengoptimasi alokasi memory virtualizer MuMu dan meningkatkan prioritas rendering grafis Vulkan/DirectX.",
        "en": "Optimizes MuMu virtualizer memory allocation and increases Vulkan/DirectX graphics rendering priority.",
        "ja": "MuMu仮想マシンのメモリ割り当てを最適化し、Vulkan/DirectXグラフィックスレンダリングの優先度を上げます。",
        "zh": "优化MuMu模拟器的内存分配，并提高Vulkan/DirectX图形渲染优先级。",
        "ar": "يُحسن تخصيص ذاكرة محاكي MuMu ويزيد من أولوية عرض الرسومات لـ Vulkan/DirectX.",
        "pt": "Otimiza a alocação de memória do emulador MuMu e aumenta a prioridade de renderização gráfica Vulkan/DirectX.",
        "es": "Optimiza la asignación de memoria del emulador MuMu y aumenta la prioridad de renderizado de gráficos Vulkan/DirectX.",
        "ru": "Оптимизирует распределение памяти виртуализатора MuMu и повышает приоритет рендеринга графики Vulkan/DirectX.",
        "de": "Optimiert die Speicherzuweisung des MuMu-Virtualisierers und erhöht die Vulkan/DirectX-Grafik-Rendering-Priorität.",
        "fr": "Optimise l'allocation de mémoire du virtualiseur MuMu et augmente la priorité de rendu graphique Vulkan/DirectX.",
        "ko": "MuMu 가상화 장치 메모리 할당을 최적화하고 Vulkan/DirectX 그래픽 렌더링 우선 순위를 높입니다.",
        "tr": "MuMu sanallaştırıcı bellek tahsisini optimize eder ve Vulkan/DirectX grafik oluşturma önceliğini artırır."
    },
    "Emu_Desc_SmartGaGa": {
        "id": "Mengurangi latency input engine Titan dan memprioritaskan alokasi CPU untuk stabilitas FPS game shooter.",
        "en": "Reduces Titan engine input latency and prioritizes CPU allocation for shooter game FPS stability.",
        "ja": "Titanエンジンの入力遅延を減らし、シューティングゲームのFPS安定性のためにCPU割り当てを優先します。",
        "zh": "降低Titan引擎的输入延迟，并优先分配CPU以实现射击游戏FPS稳定。",
        "ar": "يقلل من انتقال إدخال محرك Titan ويعطي الأولوية لتخصيص المعالج لاستقرار FPS في ألعاب إطلاق النار.",
        "pt": "Reduz a latência de entrada do mecanismo Titan e prioriza a alocação de CPU para estabilidade de FPS do jogo de tiro.",
        "es": "Reduce la latencia de entrada del motor Titan y prioriza la asignación de CPU para la estabilidad de FPS en juegos de disparos.",
        "ru": "Снижает задержку ввода движка Titan и приоритизирует выделение ядер процессора для стабильности FPS в шутерах.",
        "de": "Reduziert die Eingabelatenz der Titan-Engine und priorisiert die CPU-Zuweisung für die FPS-Stabilität von Shooter-Spielen.",
        "fr": "Réduit la latence d'entrée du moteur Titan et priorise l'allocation CPU pour la stabilité du FPS dans les tirs.",
        "ko": "Titan 엔진의 입력 지연 시간을 줄이고 슈팅 게임 FPS 안정성을 위해 CPU 할당을 우선시합니다.",
        "tr": "Titan motoru giriş gecikmesini azaltır ve nişancı oyunu FPS kararlılığı için CPU tahsisine öncelik verir."
    }
}

# Update translations.json dict
for key, translations in new_keys_data.items():
    for lang, val in translations.items():
        if lang in data:
            data[lang][key] = val

with open(json_path, 'w', encoding='utf-8') as f:
    json.dump(data, f, indent=2, ensure_ascii=False)

print("Successfully updated translations.json!")
