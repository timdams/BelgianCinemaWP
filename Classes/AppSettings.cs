using System;
using System.IO.IsolatedStorage;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Belgian_Cinema.Model;

namespace Belgian_Cinema.Classes
{
 
        public class AppSettings
        {
            // Our isolated storage settings
            IsolatedStorageSettings settings;

            // The isolated storage key names of our settings
            const string LanguageSettingKeyName = "LanguageSetting";
            const string CinemaSettingKeyName = "CinemaSetting";
            

            // The default value of our settings
            const string LanguageSettingDefault = "fr";
            
            

            /// <summary>
            /// Constructor that gets the application settings.
            /// </summary>
            public AppSettings()
            {
                // Get the settings for this application.
                settings = IsolatedStorageSettings.ApplicationSettings;
            }

            /// <summary>
            /// Update a setting value for our application. If the setting does not
            /// exist, then add the setting.
            /// </summary>
            /// <param name="Key"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            public bool AddOrUpdateValue(string Key, Object value)
            {
                bool valueChanged = false;

                // If the key exists
                if (settings.Contains(Key))
                {
                    // If the value has changed
                    if (settings[Key] != value)
                    {
                        // Store the new value
                        settings[Key] = value;
                        valueChanged = true;
                    }
                }
                // Otherwise create the key.
                else
                {
                    settings.Add(Key, value);
                    valueChanged = true;
                }
                return valueChanged;
            }

            /// <summary>
            /// Get the current value of the setting, or if it is not found, set the 
            /// setting to the default setting.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="Key"></param>
            /// <param name="defaultValue"></param>
            /// <returns></returns>
            public T GetValueOrDefault<T>(string Key, T defaultValue)
            {
                T value;

                // If the key exists, retrieve the value.
                if (settings.Contains(Key))
                {
                    value = (T)settings[Key];
                }
                // Otherwise, use the default value.
                else
                {
                    value = defaultValue;
                }
                return value;
            }

            /// <summary>
            /// Save the settings.
            /// </summary>
            public void Save()
            {
                settings.Save();
            }




            /// <summary>
            /// Property to get and set a ListBox Setting Key.
            /// </summary>
            public Cinema CinemaSetting
            {
                get
                {
                    return GetValueOrDefault<Cinema>(CinemaSettingKeyName, new Cinema("UGC Antwerpen", "786"));
                }
                set
                {
                    if (AddOrUpdateValue(CinemaSettingKeyName, value))
                    {
                        Save();
                    }
                }
            }

            public string LanguageSetting
            {
                get
                {
                    return GetValueOrDefault<string>(LanguageSettingKeyName, LanguageSettingDefault);
                }
                set
                {
                    if (AddOrUpdateValue(LanguageSettingKeyName, value))
                    {
                        Save();
                    }
                }
            }



        }
    
}
