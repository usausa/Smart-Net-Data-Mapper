namespace DataAccess.FormsApp.Components
{
    using System;
    using System.Threading.Tasks;

    using Xamarin.Forms;

    public sealed class Dialogs : IDialogs
    {
        public async Task<bool> Confirm(string message, string title, string acceptButton, string cancelButton)
        {
            return await Application.Current.MainPage.DisplayAlert(title, message, acceptButton, cancelButton);
        }

        public async Task Information(string message, string title, string cancelButton)
        {
            await Application.Current.MainPage.DisplayAlert(title, message, cancelButton);
        }
    }
}
