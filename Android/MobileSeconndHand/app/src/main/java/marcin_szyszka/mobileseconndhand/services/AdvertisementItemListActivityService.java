package marcin_szyszka.mobileseconndhand.services;

/**
 * Created by marcianno on 2016-03-30.
 */
public class AdvertisementItemListActivityService {
    private static AdvertisementItemListActivityService ourInstance = new AdvertisementItemListActivityService();

    public static AdvertisementItemListActivityService getInstance() {
        return ourInstance;
    }

    private AdvertisementItemListActivityService() {
    }
}
