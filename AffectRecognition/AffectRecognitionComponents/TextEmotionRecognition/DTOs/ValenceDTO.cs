using System;
namespace TextEmotionRecognition.DTOs
{
    [Serializable]
    public class ValenceDTO
    {
        public string Valence { get; set; }
        public float Score { get; set; }
    }
}
