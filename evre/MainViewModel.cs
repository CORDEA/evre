using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using Google;

namespace evre;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly Authorizer _authorizer;
    private readonly HasOngoingEventUseCase _hasOngoingEventUseCase;
    private readonly RemoveOngoingEventUseCase _removeOngoingEventUseCase;
    private readonly StartEventUseCase _startEventUseCase;
    private readonly StopEventUseCase _stopEventUseCase;
    private string _description = "";
    private bool _isRemoveButtonVisible;
    private string _name = "";
    private UpdateButtonText _updateButtonText = UpdateButtonText.Start;

    public MainViewModel(Authorizer authorizer,
        HasOngoingEventUseCase hasOngoingEventUseCase,
        StartEventUseCase registerEventUseCase,
        StopEventUseCase updateOngoingEventUseCase,
        RemoveOngoingEventUseCase removeOngoingEventUseCase)
    {
        _authorizer = authorizer;
        _hasOngoingEventUseCase = hasOngoingEventUseCase;
        _startEventUseCase = registerEventUseCase;
        _stopEventUseCase = updateOngoingEventUseCase;
        _removeOngoingEventUseCase = removeOngoingEventUseCase;
        UpdateButtonState(_hasOngoingEventUseCase.Execute());
    }

    public ICommand UpdateEventCommand => new Command(UpdateEvent);
    public ICommand RemoveEventCommand => new Command(RemoveEvent);

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

    public UpdateButtonText UpdateButtonText
    {
        get => _updateButtonText;
        private set => SetField(ref _updateButtonText, value);
    }

    public bool IsRemoveButtonVisible
    {
        get => _isRemoveButtonVisible;
        private set => SetField(ref _isRemoveButtonVisible, value);
    }


    public event PropertyChangedEventHandler PropertyChanged;


    private async void UpdateEvent()
    {
        await _authorizer.Authorize();
        var now = DateTime.Now;
        if (_hasOngoingEventUseCase.Execute())
        {
            try
            {
                await _stopEventUseCase.Execute(now);
            }
            catch (GoogleApiException e)
            {
                await Toast.Make(e.Message).Show();
                return;
            }

            UpdateButtonState(false);
            await Toast.Make("The event has been updated.").Show();
            return;
        }

        if (string.IsNullOrWhiteSpace(Name)) return;
        try
        {
            await _startEventUseCase.Execute(Name, Description, now);
        }
        catch (GoogleApiException e)
        {
            await Toast.Make(e.Message).Show();
            return;
        }

        UpdateButtonState(true);
        await Toast.Make("The event has been updated.").Show();
    }

    private async void RemoveEvent()
    {
        await _authorizer.Authorize();
        try
        {
            await _removeOngoingEventUseCase.Execute();
        }
        catch (GoogleApiException e)
        {
            await Toast.Make(e.Message).Show();
            return;
        }

        UpdateButtonState(false);
        await Toast.Make("The event has been removed.").Show();
    }

    private void UpdateButtonState(bool hasOngoingEvent)
    {
        UpdateButtonText = hasOngoingEvent ? UpdateButtonText.Stop : UpdateButtonText.Start;
        IsRemoveButtonVisible = hasOngoingEvent;
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

public enum UpdateButtonText
{
    Start,
    Stop
}
