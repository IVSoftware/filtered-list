using IVSoftware.Portable;
using QtimeUniversal.BusinessEntities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using static IVSoftware.Portable.Reconciler;

namespace filtered_list
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            // Initialize for test purposes
            BindingContext.AllProjects.Add(new Project { Description = "Cosmic Voyage: Interstellar Exploration" });
            BindingContext.AllProjects.Add(new Project { Description = "EcoWorld: Sustainable Ecosystem Restoration" });
            BindingContext.AllProjects.Add(new Project { Description = "Quantum Odyssey: Advanced Physics Research" });
            BindingContext.AllProjects.Add(new Project { Description = "Mars Terraformers: Red Planet Transformation" });
            BindingContext.AllProjects.Add(new Project { Description = "AquaHarvest: Oceanic Food Sustainability" });
            BindingContext.AllProjects.Add(new Project { Description = "TimeWarp Chronicles: Temporal Anomaly Investigation" });
            BindingContext.AllProjects.Add(new Project { Description = "Biomech Revolution: Bioengineering Marvels" });
            BindingContext.AllProjects.Add(new Project { Description = "Celestial Harmony: Space Civilization Diplomacy" });
            BindingContext.AllProjects.Add(new Project { Description = "Nebula Dreamscape: Cosmic Artistic Expression" });
            BindingContext.AllProjects.Add(new Project { Description = "CryoGenesis: Cryonics and Future Resurrection" });

            BindingContext.FilterText = string.Empty;
        }
        new MainPageBindingContext BindingContext =>(MainPageBindingContext)base.BindingContext;
    }

    class MainPageBindingContext : INotifyPropertyChanged
    {
        public ObservableCollection<Project> FilteredProjects{ get; } = new ObservableCollection<QtimeUniversal.BusinessEntities.Project>();
        public ObservableCollection<Project> AllProjects{ get; } = new ObservableCollection<QtimeUniversal.BusinessEntities.Project>();
        public string FilterText
        {
            get => _filterText;
            set
            {
                if (!Equals(_filterText, value))
                {
                    _filterText = value;
                    OnPropertyChanged();
                }
            }
        }
        string _filterText = null;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            switch (propertyName) 
            {
                case nameof(FilterText):
                    OnFilterTextChanged();
                    break;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnFilterTextChanged()
        {
            var preFilterProjects = 
                string.IsNullOrWhiteSpace(FilterText) ? 
                    AllProjects : 
                    AllProjects.Where(_=>_.Description.Contains(FilterText, StringComparison.OrdinalIgnoreCase));

            var reconciled = Reconciler.Reconcile(
                srceA: FilteredProjects, 
                srceB: preFilterProjects,
                uidSorter: (a,b)=> (CompareUIDResult)a.Description.CompareTo(b.Description),
                versionComparer: (a,b)=> CompareVersionResult.Equal
            );
            foreach (var remove in reconciled.OnlyInA)
            {
                FilteredProjects.Remove(remove);
            }
            foreach (var add in reconciled.OnlyInB)
            {
                FilteredProjects.Add(add);
            }
        }
    }
}

namespace QtimeUniversal.BusinessEntities
{
    class Project : INotifyPropertyChanged
    {
        public string Description
        {
            get => _description;
            set
            {
                if (!Equals(_description, value))
                {
                    _description = value;
                    OnPropertyChanged();
                }
            }
        }
        string _description = string.Empty;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
