using System.Windows;
using System.Windows.Controls;
using PCOptimizer.Services;

namespace PCOptimizer.Views
{
    public partial class AboutPage : Page
    {
        public AboutPage()
        {
            InitializeComponent();
            Loaded += AboutPage_Loaded;
        }

        private void AboutPage_Loaded(object sender, RoutedEventArgs e)
        {
            ApplyLanguageTranslations();
        }

        private void ApplyLanguageTranslations()
        {
            PageTitleText.Text = TranslationService.Get("About_Title");
            PageSubtitleText.Text = TranslationService.Get("About_Subtitle");
            CreatorLabel.Text = TranslationService.Get("About_Creator");
            CreatorDesc.Text = TranslationService.Get("About_CreatorDesc");
            FounderLabel.Text = TranslationService.Get("About_Founder");
            FounderDesc.Text = TranslationService.Get("About_FounderDesc");
            SalesLabel.Text = TranslationService.Get("About_Sales");
            SalesDesc.Text = TranslationService.Get("About_SalesDesc");
            PromotionLabel.Text = TranslationService.Get("About_Promotion");
            PromotionDesc.Text = TranslationService.Get("About_PromotionDesc");
            VisionTitle.Text = TranslationService.Get("About_VisionTitle");
            VisionDesc.Text = TranslationService.Get("About_VisionDesc");
            ThankYouText.Text = TranslationService.Get("About_ThankYou");
            MadeByText.Text = TranslationService.Get("About_MadeBy");
        }
    }
}
