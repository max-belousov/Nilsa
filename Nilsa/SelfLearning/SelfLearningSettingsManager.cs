using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Nilsa;

public class SelfLearningSettingsManager
{
    private readonly string _settingsFilePath;
    private readonly int _algorithmId;

    public bool ReferenceBase { get; set; }
    public bool AllBase { get; set; }
    public bool NoBase { get; set; }

    public SelfLearningSettingsManager(int algorithmId)
    {
        _algorithmId = algorithmId;
        _settingsFilePath = Path.Combine(FormMain.sDataPath, $"_selflearning_settings_alg{Convert.ToString(algorithmId)}.values");
        LoadSettings();
    }

    public void UpdateSettings(bool referenceBase, bool allBase, bool noBase)
    {
        ReferenceBase = referenceBase;
        AllBase = allBase;
        NoBase = noBase;

        SaveSettings();
    }

    public void RemoveSettings()
    {
        ReferenceBase = false;
        AllBase = false;
        NoBase = false;

        SaveSettings();
    }

    private void LoadSettings()
    {
        if (File.Exists(_settingsFilePath))
        {
            var json = File.ReadAllText(_settingsFilePath);
            var settings = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

            if (settings.TryGetValue("ReferenceBase", out var referenceBase) && referenceBase is bool)
                ReferenceBase = (bool)referenceBase;

            if (settings.TryGetValue("AllBase", out var allBase) && allBase is bool)
                AllBase = (bool)allBase;

            if (settings.TryGetValue("NoBase", out var noBase) && noBase is bool)
                NoBase = (bool)noBase;
        }
        else
        {
            ReferenceBase = false;
            AllBase = false;
            NoBase = false;
        }
    }

    private void SaveSettings()
    {
        var settings = new Dictionary<string, object>
        {
            { "AlgorithmId", _algorithmId },
            { "ReferenceBase", ReferenceBase },
            { "AllBase", AllBase },
            { "NoBase", NoBase }
        };

        var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
        File.WriteAllText(_settingsFilePath, json);
    }
}