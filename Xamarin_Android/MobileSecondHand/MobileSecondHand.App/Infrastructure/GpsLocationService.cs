using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using MobileSecondHand.App.Consts;
using MobileSecondHand.Models.Location;
using MobileSecondHand.Models.Settings;
using MobileSecondHand.Services.Location;

namespace MobileSecondHand.App.Infrastructure
{
	public class GpsLocationService : Service, ILocationListener, IDialogInterfaceOnClickListener
	{
		private bool isGPSEnabled;
		private bool isNetworkEnabled;
		private double latitude;
		private double longitude;
		private Location location;
		private LocationManager locationManager;
		private Context mContext;
		SharedPreferencesHelper sharedPreferencesHelper;
		ISettingWindowCloseListener settingWindowListener;
		private long MIN_DISTANCE_CHANGE_FOR_UPDATES = 500;//500m
		static GpsLocationService serviceInstance;

		private long MIN_TIME_BW_UPDATES = 1000 * 60 * 1; // 1 minute

		public bool CanGetLocation { get; private set; }

		public static GpsLocationService GetServiceInstance(Context context, ISettingWindowCloseListener listener = null)
		{
			if (serviceInstance == null)
			{
				serviceInstance = new GpsLocationService(context, listener);
			}

			return serviceInstance;
		}

		public GpsLocationService(Context context, ISettingWindowCloseListener settingWindowListener = null)
		{
			this.mContext = context;
			this.settingWindowListener = settingWindowListener;
			this.sharedPreferencesHelper = new SharedPreferencesHelper(context);
			this.location = GetLocation();
		}

		public Location GetLocation()
		{
			try
			{
				locationManager = (LocationManager)mContext.GetSystemService("location");

				// Getting GPS status
				isGPSEnabled = locationManager.IsProviderEnabled(LocationManager.GpsProvider);

				// Getting network status
				isNetworkEnabled = locationManager.IsProviderEnabled(LocationManager.NetworkProvider);

				if (!isGPSEnabled && !isNetworkEnabled)
				{
					//showalert in activity
				}
				else
				{
					this.CanGetLocation = true;
					if (isNetworkEnabled)
					{
						locationManager.RequestLocationUpdates(
								LocationManager.NetworkProvider,
								MIN_TIME_BW_UPDATES,
								MIN_DISTANCE_CHANGE_FOR_UPDATES, this);
						if (locationManager != null)
						{
							location = locationManager.GetLastKnownLocation(LocationManager.NetworkProvider);
							if (location != null)
							{
								latitude = location.Latitude;
								longitude = location.Longitude;
							}
						}
					}
					if (isGPSEnabled)
					{
						if (location == null)
						{
							locationManager.RequestLocationUpdates(
									LocationManager.GpsProvider,
									MIN_TIME_BW_UPDATES,
									MIN_DISTANCE_CHANGE_FOR_UPDATES, this);
							if (locationManager != null)
							{
								location = locationManager.GetLastKnownLocation(LocationManager.GpsProvider);
								if (location != null)
								{
									latitude = location.Latitude;
									longitude = location.Longitude;
								}
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				AlertsService.ShowToast(this.mContext, "Nie masz w³¹czonej lokalizacji!");
			}

			return location;
		}

		public CoordinatesForAdvertisementsModel GetCoordinatesModel()
		{
			CoordinatesForAdvertisementsModel coordinatesModel = new CoordinatesForAdvertisementsModel();
			var location = GetLocation();
			var settingsMOdel = (AppSettingsModel)sharedPreferencesHelper.GetSharedPreference<AppSettingsModel>(SharedPreferencesKeys.APP_SETTINGS);
			if (location == null || latitude == 0 || longitude == 0)
			{
				AlertsService.ShowToast(this.mContext, "Nie mogê ustaliæ aktualnej lokalizacji. SprawdŸ ustawienia GPS");
				throw new Exception("Brak lokalizacji");
			}
			else
			{
				coordinatesModel.Latitude = latitude;
				coordinatesModel.Longitude = longitude;
				if (settingsMOdel != null)
				{
					coordinatesModel.MaxDistance = settingsMOdel.LocationSettings.MaxDistance;
				}
				else
				{
					coordinatesModel.MaxDistance = 500;
				}
			}

			return coordinatesModel;
		}

		public void ShowSettingsAlert()
		{
			AlertDialog.Builder alertDialog = new AlertDialog.Builder(mContext);
			alertDialog.SetTitle("Ustawienia lokalizacji");
			alertDialog.SetMessage("GPS jest wy³¹czony. Czy chcesz teraz przejœæ do ustawieñ aby w³¹czyæ lokalizacjê?");

			alertDialog.SetPositiveButton("PrzejdŸ do ustawieñ", this);
			alertDialog.SetNegativeButton("Anuluj", this);
			alertDialog.SetCancelable(false);
			alertDialog.Show();
		}


		public void OnClick(IDialogInterface dialog, int which)
		{
			if (which == -1)
			{
				//ok
				Intent intent = new Intent(Settings.ActionLocationSourceSettings);
				mContext.StartActivity(intent);
			}
			else
			{
				settingWindowListener.OnSettingsWindowClose();
			}

		}

		public override IBinder OnBind(Intent intent)
		{
			return null;
		}

		public void OnLocationChanged(Location location)
		{
			//throw new NotImplementedException();
		}

		public void OnProviderDisabled(string provider)
		{
			//throw new NotImplementedException();
		}

		public void OnProviderEnabled(string provider)
		{
			//throw new NotImplementedException();
		}

		public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
		{
			//throw new NotImplementedException();
		}
	}


}