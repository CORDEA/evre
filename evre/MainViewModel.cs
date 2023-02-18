using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Google;
using Google.Apis.Calendar.v3.Data;

namespace evre;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly Authorizer _authorizer;
    private readonly HasOngoingEventUseCase _hasOngoingEventUseCase;
    private readonly RegisterEventUseCase _registerEventUseCase;
    private readonly UpdateOngoingEventUseCase _updateOngoingEventUseCase;
    private string _buttonText = "";
    private string _description = "";
    private string _name = "";

    public MainViewModel(Authorizer authorizer,
        HasOngoingEventUseCase hasOngoingEventUseCase,
        RegisterEventUseCase registerEventUseCase,
        UpdateOngoingEventUseCase updateOngoingEventUseCase)
    {
        _authorizer = authorizer;
        _hasOngoingEventUseCase = hasOngoingEventUseCase;
        _registerEventUseCase = registerEventUseCase;
        _updateOngoingEventUseCase = updateOngoingEventUseCase;
        ButtonText = _hasOngoingEventUseCase.Execute() ? "Stop" : "Start";
    }

    public ICommand AddEventCommand => new Command(RegisterEvent);

    public string Name
    {
        get => _name;
        set => SetField(ref _name, value);
    }

    public string Description
    {
        get => _description;
        set => SetField(ref _description, value);
    }

    public string ButtonText
    {
        get => _buttonText;
        private set => SetField(ref _buttonText, value);
    }

    public event PropertyChangedEventHandler PropertyChanged;


    private async void RegisterEvent()
    {
        await _authorizer.Authorize();
        var now = DateTime.Now;
        if (_hasOngoingEventUseCase.Execute())
        {
            try
            {
                await _updateOngoingEventUseCase.Execute(
                    new Event
                    {
                        End = new EventDateTime
                        {
                            DateTime = now
                        }
                    }
                );
            }
            catch (GoogleApiException e)
            {
                // TODO
                return;
            }

            ButtonText = "Start";
            return;
        }

        if (string.IsNullOrWhiteSpace(Name)) return;
        try
        {
            await _registerEventUseCase.Execute(
                new Event
                {
                    Summary = Name,
                    Description = Description,
                    Start = new EventDateTime
                    {
                        DateTime = now
                    },
                    End = new EventDateTime
                    {
                        DateTime = now.Add(TimeSpan.FromMinutes(10))
                    }
                }
            );
        }
        catch (GoogleApiException e)
        {
            // TODO
            return;
        }

        ButtonText = "Stop";
    }

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
