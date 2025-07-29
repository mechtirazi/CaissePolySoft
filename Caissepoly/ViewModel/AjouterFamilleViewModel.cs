using CaissePoly.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace CaissePoly.ViewModel
{
    public class AjouterFamilleViewModel : INotifyPropertyChanged
    {
        private string nomFamille;
        private ObservableCollection<Famille> _existingFamilies;
        private Famille _selectedFamille;

        public string NomFamille
        {
            get => nomFamille;
            set { nomFamille = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Famille> ExistingFamilies
        {
            get => _existingFamilies;
            set { _existingFamilies = value; OnPropertyChanged(); }
        }

        public Famille SelectedFamille
        {
            get => _selectedFamille;
            set { _selectedFamille = value; OnPropertyChanged(); }
        }

        public ICommand AjouterFamilleCommand { get; }
        public ICommand SupprimerFamilleCommand { get; }
        public ICommand ModifierFamilleCommand { get; }

        public AjouterFamilleViewModel()
        {
            AjouterFamilleCommand = new RelayCommand(AjouterFamille);
            SupprimerFamilleCommand = new RelayCommand<Famille>(SupprimerFamille);
            ModifierFamilleCommand = new RelayCommand<Famille>(ModifierFamille);
            LoadExistingFamilies();
        }

        private void LoadExistingFamilies()
        {
            try
            {
                using (var context = new CDBContext())
                {
                    ExistingFamilies = new ObservableCollection<Famille>(
                        context.Famille.ToList()
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des familles : {ex.Message}");
                ExistingFamilies = new ObservableCollection<Famille>();
            }
        }

        private void AjouterFamille()
        {
            if (string.IsNullOrWhiteSpace(NomFamille))
            {
                MessageBox.Show("Le nom de la famille est requis.");
                return;
            }

            using (var context = new CDBContext())
            {
                if (context.Famille.Any(f => f.nomF.ToLower() == NomFamille.ToLower()))
                {
                    MessageBox.Show("Cette famille existe déjà !");
                    return;
                }

                var nouvelleFamille = new Famille { nomF = NomFamille };
                context.Famille.Add(nouvelleFamille);
                context.SaveChanges();

                // Ajouter localement après ajout DB
                ExistingFamilies.Add(nouvelleFamille);
            }

            MessageBox.Show("Famille ajoutée avec succès !");
            NomFamille = string.Empty;
        }

        private void SupprimerFamille(Famille famille)
        {
            if (famille == null) return;

            var result = MessageBox.Show($"Supprimer la famille '{famille.nomF}' ?", "Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                using (var context = new CDBContext())
                {
                    var toDelete = context.Famille.FirstOrDefault(f => f.idF == famille.idF);
                    if (toDelete != null)
                    {
                        context.Famille.Remove(toDelete);
                        context.SaveChanges();
                    }
                }

                ExistingFamilies.Remove(famille);
            }
        }

        private void ModifierFamille(Famille famille)
        {
            if (famille == null) return;

            var nouveauNom = Microsoft.VisualBasic.Interaction.InputBox("Modifier le nom de la famille :", "Modifier", famille.nomF);

            if (!string.IsNullOrWhiteSpace(nouveauNom))
            {
                using (var context = new CDBContext())
                {
                    var toUpdate = context.Famille.FirstOrDefault(f => f.idF == famille.idF);
                    if (toUpdate != null)
                    {
                        toUpdate.nomF = nouveauNom;
                        context.SaveChanges();
                        LoadExistingFamilies(); // Recharge depuis la base après modification


                    }
                }

                // Mise à jour locale
                famille.nomF = nouveauNom;
                OnPropertyChanged(nameof(ExistingFamilies));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
