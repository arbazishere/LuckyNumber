using UnityEngine;

namespace ArbazAminKhan.SaveLoadSystems.Datas
{
    public class SaveLoad_Data { }

    public class SaveLoadVariable<T>
    {
        private T _value;
        private readonly string _key;

        public SaveLoadVariable(string key, T defaultValue)
        {
            _key = key;
            _value = defaultValue;
        }

        public T value
        {
            get => _value;
            set
            {
                _value = value;

                if (value is float floatValue)
                    PlayerPrefs.SetFloat(_key, floatValue);
                else if (value is int intValue)
                    PlayerPrefs.SetInt(_key, intValue);
            }
        }

        public void LoadValue()
        {
            if (!PlayerPrefs.HasKey(_key))
                return;

            if (typeof(T) == typeof(float))
                _value = (T)(object)PlayerPrefs.GetFloat(_key);
            else if (typeof(T) == typeof(int))
                _value = (T)(object)PlayerPrefs.GetInt(_key);
        }
    }
}