## Filtered List

A minimal scheme to realize this could involve two observable collections of `Project`: The `ItemsSource` is called `FilteredProjects` and is bound to the `CollectionView`. A second OC will contain "all" the projects, and isn't bound to a UI element. However (for example) you could add a `CollectionChanged` handler to it to determine whether changes to the underlying `AllProjects` collection are relevant in terms of the current filter.

The basic idea is to watch changes made to the `FilterText` property made by the `Entry` control, preview what items _should be_ visible, and then reconcile the two lists in what is hopefully a decently efficient and quick manner (the 'Reconciler' class). I've made a [repo]() if seeing the full code would help.

```csharp
class MainPageBindingContext : INotifyPropertyChanged
{
    // Source of the filtered items in the list 
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
```



