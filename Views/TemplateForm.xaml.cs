using IdentityCardGenerator.ViewModels;
using Microsoft.Maui.Controls;

namespace IdentityCardGenerator.Views
{
    public partial class TemplateForm : ContentPage
    {
        public TemplateForm(TemplateViewModel templateViewModel)
        {
            InitializeComponent();
            BindingContext = templateViewModel;
            graphics.Drawable = new RoundedHexagonDrawable();
        }
    }
}