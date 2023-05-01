using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalInfo.Models
{
    internal class FestItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Creator { get; set; }
        public string SpatialCoverage { get; set; }
        public string TemporalCoverage { get; set; }
        public string SubjectCategory { get; set; }
        public string Url { get; set; }
        public string CollectionDb { get; set; }
        public string ReferenceIdentifier { get; set; }     // 썸네일 이미지
        public string subDescription { get; set; }          // 보조 서술

    }
}
