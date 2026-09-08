using Exiled.API.Interfaces;
using System.ComponentModel;

namespace SCP034
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        public float MinDamage { get; set; } = 7f;
        public float MaxDamage { get; set; } = 15f;
        [Description("Значение от 0 до 100")]
        public int ChanceToWork { get; set; } = 90;

        [Description("Максимальное время превращения")]
        public float MaxTransformTime = 180;
    }
}
