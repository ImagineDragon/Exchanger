using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Exchanger.Models
{
    public class FileModel
    {
        [Required]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "File name")]
        public string FileName { get; set; }

        [Required]
        [Display(Name = "Size")]
        public int Size { get; set; }

        [Required]
        [Display(Name = "Creation date")]
        public DateTime CreationDate { get; set; }

        [Required]
        [Display(Name = "Last edit date")]
        public DateTime LastEditDate { get; set; }

        [Required]
        [Display(Name = "Created by")]
        public string CreatedBy { get; set; }

        [Required]
        [Display(Name = "Access")]
        public string Access { get; set; }

        [Required]
        [Display(Name = "File type")]
        public string FileType { get; set; }
    }

    public class EditFileModel
    {
        [Required]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "File name")]
        public string FileName { get; set; }

        [Required]
        [Display(Name = "Size")]
        public int Size { get; set; }

        [Required]
        [Display(Name = "Creation date")]
        public DateTime CreationDate { get; set; }

        [Required]
        [Display(Name = "Last edit date")]
        public DateTime LastEditDate { get; set; }

        [Required]
        [Display(Name = "Created by")]
        public string CreatedBy { get; set; }

        [Required]
        [Display(Name = "Access")]
        public List<string> Access { get; set; }

        [Required]
        [Display(Name = "File type")]
        public string FileType { get; set; }
    }

    public class ShareFileModel
    {
        [Required]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "File name")]
        public string FileName { get; set; }

        [Required]
        [Display(Name = "Login")]
        public IEnumerable<string> Login { get; set; }
    }

    public class ShareModel
    {
        [Required]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "File name")]
        public string FileName { get; set; }

        [Required]
        [Display(Name = "Login")]
        public string Login { get; set; }
    }
}