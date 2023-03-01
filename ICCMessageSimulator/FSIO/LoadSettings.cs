using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LoadSettings
{
    private static LoadSettings loadSetting = null;
    public static LoadSettings Instance
    {
        get
        {
            if (loadSetting == null)
            {
                loadSetting = new LoadSettings();
            }
            return loadSetting;
        }
        private set { loadSetting = value; }
    }
    private IniFile _iniFile = null;
    private IniFile iniFile
    {
        get
        {
            if(_iniFile == null)
            {
                _iniFile = new IniFile();
            }
            return _iniFile;
        }
        set { _iniFile = value; }
    }
    private SettingsProfile _settingsProfile = null;
    public SettingsProfile settingsProfile
    {
        get
        {
            if(_settingsProfile == null)
            {
                _settingsProfile = new SettingsProfile();
            }
            return _settingsProfile;
        }
        private set { _settingsProfile = value; }
    }

    public void Load_Settings(string path,string filePath)
    {
        if (!Directory.Exists(IniFile.defaultFolderPath))
        {
            Directory.CreateDirectory(IniFile.defaultFolderPath);
        }
        if (!File.Exists(IniFile.defaultFilePath))
        {
            iniFile.WriteClassDataToSection(settingsProfile);
            iniFile.Save();
        }
        iniFile.Load(filePath);
        iniFile.LoadClassDataFromSection(settingsProfile);
    }
    public void Save_Settings(string path, string filePath)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        if (!File.Exists(path))
        {
            iniFile.WriteClassDataToSection(settingsProfile);
            iniFile.Save(filePath);
        }
    }
}
