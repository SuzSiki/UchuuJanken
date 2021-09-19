using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class SoundSetter:MonoBehaviour
{
    [SerializeField] AudioMixer mixer;
    [SerializeField] SliderSetting[] settings;

    void Start()
    {
        foreach(var setting in settings)
        {
            mixer.GetFloat(setting.target.ToString(),out float volume);
            setting.slider.value = (volume+80)/100;
            setting.slider.onValueChanged.AddListener((x)=>ChangeValue(setting.target,x));
        }
    }

    void ChangeValue(SoundType type,float value)
    {
        var volume = value*100-80;
        mixer.SetFloat(type.ToString(),volume);
    }

    [System.Serializable]
    class SliderSetting
    {
        public Slider slider;
        public SoundType target;
    }
    enum SoundType
    {
        Master,
        BGM,
        SE
    }
}