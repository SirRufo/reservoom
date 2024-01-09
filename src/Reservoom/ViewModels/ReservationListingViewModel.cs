using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Reservoom.Commands;
using Reservoom.Models;
using Reservoom.Services;
using Reservoom.Stores;

namespace Reservoom.ViewModels
{
    public class ReservationListingViewModel : ViewModelBase
    {
        private readonly HotelStore _hotelStore;

        private readonly ObservableCollection<ReservationViewModel> _reservations;

        public IEnumerable<ReservationViewModel> Reservations => _reservations;

        public bool HasReservations => _reservations.Any();

        [Reactive] public string ErrorMessage { get; set; }

        public bool HasErrorMessage => !string.IsNullOrEmpty( ErrorMessage );

        [Reactive] public bool IsLoading { get; set; }

        public ICommand LoadReservationsCommand { get; }
        public ICommand MakeReservationCommand { get; }

        public ReservationListingViewModel( HotelStore hotelStore, NavigationService<MakeReservationViewModel> makeReservationNavigationService )
        {
            _hotelStore = hotelStore;
            _reservations = new ObservableCollection<ReservationViewModel>();

            LoadReservationsCommand = new LoadReservationsCommand( this, hotelStore );
            MakeReservationCommand = new NavigateCommand<MakeReservationViewModel>( makeReservationNavigationService );

            _hotelStore.ReservationMade += OnReservationMode;
            _reservations.CollectionChanged += OnReservationsChanged;
        }

        public override void Dispose()
        {
            _hotelStore.ReservationMade -= OnReservationMode;
            base.Dispose();
        }

        private void OnReservationMode( Reservation reservation )
        {
            ReservationViewModel reservationViewModel = new ReservationViewModel( reservation );
            _reservations.Add( reservationViewModel );
        }

        public static ReservationListingViewModel LoadViewModel( HotelStore hotelStore, NavigationService<MakeReservationViewModel> makeReservationNavigationService )
        {
            ReservationListingViewModel viewModel = new ReservationListingViewModel( hotelStore, makeReservationNavigationService );

            viewModel.LoadReservationsCommand.Execute( null );

            return viewModel;
        }

        public void UpdateReservations( IEnumerable<Reservation> reservations )
        {
            _reservations.Clear();

            foreach ( Reservation reservation in reservations )
            {
                ReservationViewModel reservationViewModel = new ReservationViewModel( reservation );
                _reservations.Add( reservationViewModel );
            }
        }

        private void OnReservationsChanged( object sender, NotifyCollectionChangedEventArgs e )
        {
            this.RaisePropertyChanged( nameof( HasReservations ) );
        }
    }
}
