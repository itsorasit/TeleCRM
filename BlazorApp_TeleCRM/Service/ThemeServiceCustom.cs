namespace BlazorApp_TeleCRM.Service
{
    public class ThemeServiceCustom
    {
        public string CurrentTheme { get; set; } = "Default"; // Default theme

        public event Action OnChange;

        public void SetTheme(string theme)
        {
            CurrentTheme = theme;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }

}
