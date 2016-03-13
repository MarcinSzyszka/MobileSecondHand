package marcin_szyszka.mobileseconndhand.models;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;

/**
 * Created by marcianno on 2016-03-07.
 */
public class NewAdvertisementItem {
    public String AdvertisementTitle;
    public String AdvertisementDescription;
    public int AdvertisementPrice;
    public boolean IsOnlyForSell;
    public double Latitude;
    public double Longitude;
    public ArrayList<String> PhotosPaths;

    public NewAdvertisementItem(){
        PhotosPaths = new ArrayList<>();
    }
}
