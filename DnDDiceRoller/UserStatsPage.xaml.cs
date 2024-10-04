using System.Text.Json;

namespace DnDDiceRoller;

public partial class UserStatsPage : ContentPage
{
    public UserStatsPage()
    {
        InitializeComponent();
    }

    private async Task SaveCharacterStatsAsync()
    {
        var stats = new CharacterStats
        {
            CharacterName = CharacterNameEntry.Text,
            Race = RaceEntry.Text,
            Class = ClassEntry.Text,
            Strength = StrengthEntry.Text,
            Dexterity = DexterityEntry.Text,
            Constitution = ConstitutionEntry.Text,
            Intelligence = IntelligenceEntry.Text,
            Wisdom = WisdomEntry.Text,
            Charisma = CharismaEntry.Text,
            ArmorClass = ArmorClassEntry.Text,
            HitPoints = HitPointsEntry.Text,
            ProficiencyBonus = ProficiencyBonusEntry.Text,
            Initiative = InitiativeEntry.Text,
            Speed = SpeedEntry.Text,
            Experience = ExperienceEntry.Text
        };

        string json = JsonSerializer.Serialize(stats);
        string fileName = Path.Combine(FileSystem.AppDataDirectory, "characterstats.json");
        await File.WriteAllTextAsync(fileName, json);
        await DisplayAlert("Success", "Character stats saved!", "OK");
    }

    // Method to load character stats
    private async Task LoadCharacterStatsAsync()
    {
        try
        {
            string fileName = Path.Combine(FileSystem.AppDataDirectory, "characterstats.json");
            if (File.Exists(fileName))
            {
                string json = await File.ReadAllTextAsync(fileName);
                var stats = JsonSerializer.Deserialize<CharacterStats>(json);

                CharacterNameEntry.Text = stats.CharacterName;
                RaceEntry.Text = stats.Race;
                ClassEntry.Text = stats.Class;
                StrengthEntry.Text = stats.Strength;
                DexterityEntry.Text = stats.Dexterity;
                ConstitutionEntry.Text = stats.Constitution;
                IntelligenceEntry.Text = stats.Intelligence;
                WisdomEntry.Text = stats.Wisdom;
                CharismaEntry.Text = stats.Charisma;
                ArmorClassEntry.Text = stats.ArmorClass;
                HitPointsEntry.Text = stats.HitPoints;
                ProficiencyBonusEntry.Text = stats.ProficiencyBonus;
                InitiativeEntry.Text = stats.Initiative;
                SpeedEntry.Text = stats.Speed;
                ExperienceEntry.Text = stats.Experience;

                await DisplayAlert("Success", "Character stats loaded!", "OK");
            }
            else
            {
                await DisplayAlert("Error", "No saved stats found.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async Task DeleteCharacterStatsAsync()
    {
        string fileName = Path.Combine(FileSystem.AppDataDirectory, "characterstats.json");
        if (File.Exists(fileName))
        {
            File.Delete(fileName);
            await DisplayAlert("Success", "Character stats deleted!", "OK");
        }
        else
        {
            await DisplayAlert("Error", "No saved stats to delete.", "OK");
        }

        // Reset all fields to blank
        CharacterNameEntry.Text = string.Empty;
        RaceEntry.Text = string.Empty;
        ClassEntry.Text = string.Empty;
        StrengthEntry.Text = string.Empty;
        DexterityEntry.Text = string.Empty;
        ConstitutionEntry.Text = string.Empty;
        IntelligenceEntry.Text = string.Empty;
        WisdomEntry.Text = string.Empty;
        CharismaEntry.Text = string.Empty;
        ArmorClassEntry.Text = string.Empty;
        HitPointsEntry.Text = string.Empty;
        ProficiencyBonusEntry.Text = string.Empty;
        InitiativeEntry.Text = string.Empty;
        SpeedEntry.Text = string.Empty;
        ExperienceEntry.Text = string.Empty;
    }
    // Method to clear all stats (without deleting saved file)
    private void ClearCharacterStats()
    {
        // Reset all fields to blank (clearing the UI inputs)
        CharacterNameEntry.Text = string.Empty;
        RaceEntry.Text = string.Empty;
        ClassEntry.Text = string.Empty;
        StrengthEntry.Text = string.Empty;
        DexterityEntry.Text = string.Empty;
        ConstitutionEntry.Text = string.Empty;
        IntelligenceEntry.Text = string.Empty;
        WisdomEntry.Text = string.Empty;
        CharismaEntry.Text = string.Empty;
        ArmorClassEntry.Text = string.Empty;
        HitPointsEntry.Text = string.Empty;
        ProficiencyBonusEntry.Text = string.Empty;
        InitiativeEntry.Text = string.Empty;
        SpeedEntry.Text = string.Empty;
        ExperienceEntry.Text = string.Empty;

        // Optional: Show confirmation message to user
        DisplayAlert("Cleared", "Character stats have been cleared.", "OK");
    }

    private async void OnSaveStatsClicked(object sender, EventArgs e)
    {
        //Button button = (Button)sender;
        //await button.ScaleTo(0.9, 50, Easing.CubicOut);
        //await button.ScaleTo(1, 50, Easing.CubicIn);
        await SaveCharacterStatsAsync();
    }

    private async void OnLoadStatsClicked(object sender, EventArgs e)
    {
        //Button button = (Button)sender;
        //await button.ScaleTo(0.9, 50, Easing.CubicOut);
        //await button.ScaleTo(1, 50, Easing.CubicIn);
        await LoadCharacterStatsAsync();
    }

    private async void OnDeleteStatsClicked(object sender, EventArgs e)
    {
        //Button button = (Button)sender;
        //await button.ScaleTo(0.9, 50, Easing.CubicOut);
        //await button.ScaleTo(1, 50, Easing.CubicIn);
        await DeleteCharacterStatsAsync();
    }
    private void OnClearStatsClicked(object sender, EventArgs e)
    {
        //Button button = (Button)sender;
        //await button.ScaleTo(0.9, 50, Easing.CubicOut);
        //await button.ScaleTo(1, 50, Easing.CubicIn);
        ClearCharacterStats();
    }
    private async void OnImageButtonPressed(object sender, EventArgs e)
    {
        var imageButton = sender as ImageButton;
        await imageButton.ScaleTo(0.9, 50);
    }

    private async void OnImageButtonReleased(object sender, EventArgs e)
    {
        var imageButton = sender as ImageButton;
        await imageButton.ScaleTo(1, 50);
    }
}

