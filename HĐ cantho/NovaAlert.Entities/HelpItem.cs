namespace NovaAlert.Entities
{
    public class HelpItem: BindableEntity
    {
        bool _isWarning;
        public bool IsWarning
        {
            get { return _isWarning; }
            set { _isWarning = value; OnPropertyChanged("IsWarning"); OnPropertyChanged("Title"); }
        }

        string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; OnPropertyChanged("Title"); }
        }

        string _content;
        public string Content
        {
            get { return _content; }
            set { _content = value; OnPropertyChanged("Content"); }
        }

        public HelpItem(bool isWarning = false)
        {
            _isWarning = isWarning;            
        }
    }
}
