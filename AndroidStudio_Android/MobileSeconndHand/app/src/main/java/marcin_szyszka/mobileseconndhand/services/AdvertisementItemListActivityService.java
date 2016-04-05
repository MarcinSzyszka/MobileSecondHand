package marcin_szyszka.mobileseconndhand.services;

import java.util.ArrayList;

import marcin_szyszka.mobileseconndhand.models.AdvertisementItemShortModel;

/**
 * Created by marcianno on 2016-03-30.
 */
public class AdvertisementItemListActivityService {
    private ArrayList<AdvertisementItemShortModel> mAdvertisementModelsList = new ArrayList<>();
    private static AdvertisementItemListActivityService ourInstance = new AdvertisementItemListActivityService();

    public static AdvertisementItemListActivityService getInstance() {
        return ourInstance;
    }

    public void storeAdvertisementItemShortModelsList(ArrayList<AdvertisementItemShortModel> advertisementModelsList){
        this.mAdvertisementModelsList = advertisementModelsList;
    }

    public ArrayList<AdvertisementItemShortModel> getStoredAdvertisementItemShortModelsList(){
        return this.mAdvertisementModelsList;
    }
}
