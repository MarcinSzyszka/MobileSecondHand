package marcin_szyszka.mobileseconndhand.services;

import android.content.res.Resources;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.support.annotation.NonNull;

/**
 * Created by marcianno on 2016-03-13.
 */
public class BitmapOperationService {
    public static Bitmap ResizeImage(String imagePath, int width, int height) {
        // Get the dimensions of the bitmap
        BitmapFactory.Options bmOptions = getBitmapOptions();
        BitmapFactory.decodeFile(imagePath, bmOptions);

        scaleImage(width, height, bmOptions);

        Bitmap bitmap = BitmapFactory.decodeFile(imagePath, bmOptions);
        return bitmap;
    }

    public static Bitmap ResizeImage(byte[] imageByteArray, int width, int height) {
// Get the dimensions of the bitmap
        BitmapFactory.Options bmOptions = getBitmapOptions();
        BitmapFactory.decodeByteArray(imageByteArray, 0, imageByteArray.length, bmOptions);

        scaleImage(width, height, bmOptions);

        Bitmap bitmap = BitmapFactory.decodeByteArray(imageByteArray, 0, imageByteArray.length, bmOptions);
        return bitmap;
    }

    public static Bitmap ResizeImage(Resources res, int resourceId, int width, int height) {
        // Get the dimensions of the bitmap
        BitmapFactory.Options bmOptions = getBitmapOptions();
        BitmapFactory.decodeResource(res, resourceId);

        scaleImage(width, height, bmOptions);

        Bitmap bitmap = BitmapFactory.decodeResource(res, resourceId);
        return bitmap;
    }

    @NonNull
    private static BitmapFactory.Options getBitmapOptions() {
        BitmapFactory.Options bmOptions = new BitmapFactory.Options();
        bmOptions.inJustDecodeBounds = true;
        return bmOptions;
    }

    private static void scaleImage(int width, int height, BitmapFactory.Options bmOptions) {
        if (width > 0 && height > 0) {
            int photoW = bmOptions.outWidth;
            int photoH = bmOptions.outHeight;

            // Determine how much to scale down the image
            int scaleFactor = Math.min(photoW / width, photoH / height);
            bmOptions.inSampleSize = scaleFactor;
        }

        // Decode the image file into a Bitmap sized to fill the View
        bmOptions.inJustDecodeBounds = false;
        bmOptions.inPurgeable = true;
    }

}
