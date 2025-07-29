using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace CaissePoly.Model
{
    public class Article : INotifyPropertyChanged
    {
        [Key]
        public int idA { get; set; }

        [Required(ErrorMessage = "La designation d'article est obligatoire")]
        public string? designation { get; set; }

        [Required(ErrorMessage = "La prix d'article est obligatoire")]
        [Range(0.01, 1000000, ErrorMessage = "Le prix doit être entre 0.01 et 1 000 000")]
        public decimal? prixunitaire { get; set; }

        [Required(ErrorMessage = "La quantite d'article est obligatoire")]
        [Range(0, int.MaxValue, ErrorMessage = "La quantité de stock doit être un nombre positif ou zéro")]
        public int? quantiteStock { get; set; }
        private int _quantiteVente;

        [Range(0, int.MaxValue, ErrorMessage = "La quantité pour vente doit être un nombre positif ou zéro")]
        public int quantiteVente

        {
            get => _quantiteVente;
            set
            {
                if (_quantiteVente != value)
                {
                    _quantiteVente = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Total));
                }
            }
        }

        public string? codeabare { get; set; }

        public int? idF { get; set; }

        [ForeignKey("idF")]
        public virtual Famille? famille { get; set; }

        public virtual ICollection<Vente> Vente { get; set; } = new List<Vente>();

        public decimal Total => quantiteVente * (prixunitaire ?? 0);


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public Article()
        {
            quantiteStock = 0;
            quantiteVente = 0;
        }
    }
}
