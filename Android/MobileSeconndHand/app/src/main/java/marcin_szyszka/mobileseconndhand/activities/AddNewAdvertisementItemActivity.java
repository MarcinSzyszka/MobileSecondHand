package marcin_szyszka.mobileseconndhand.activities;

import android.app.ProgressDialog;
import android.content.Intent;
import android.content.res.Configuration;
import android.graphics.Bitmap;
import android.graphics.Point;
import android.net.Uri;
import android.os.Bundle;
import android.os.Environment;
import android.provider.MediaStore;
import android.support.v4.app.FragmentActivity;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.RadioButton;
import android.widget.TextView;
import android.widget.Toast;

import com.google.gson.Gson;

import org.json.JSONArray;
import org.json.JSONObject;

import java.io.File;
import java.io.IOException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;

import marcin_szyszka.mobileseconndhand.R;
import marcin_szyszka.mobileseconndhand.common.AppConstant;
import marcin_szyszka.mobileseconndhand.common.IAddNewAdvertisementItemPhotosRequestFinished;
import marcin_szyszka.mobileseconndhand.common.IJsonObjectReceiveDelegate;
import marcin_szyszka.mobileseconndhand.models.AdvertisementItemPhotosPaths;
import marcin_szyszka.mobileseconndhand.models.ErrorResponse;
import marcin_szyszka.mobileseconndhand.models.NewAdvertisementItem;
import marcin_szyszka.mobileseconndhand.services.AdvertisementItemsService;
import marcin_szyszka.mobileseconndhand.services.BitmapOperationService;
import marcin_szyszka.mobileseconndhand.services.GpsLocationService;

public class AddNewAdvertisementItemActivity extends FragmentActivity implements IAddNewAdvertisementItemPhotosRequestFinished, IJsonObjectReceiveDelegate {
    String mCurrentPhotoPath;
    ImageView mPhotoView1;
    private String mPhotoPath;
    Button mButtonTakePicture;
    View focusView;
    EditText advertisementTitle;
    EditText advertisementDescription;
    EditText advertisementPrice;
    private ProgressDialog progress;
    GpsLocationService gps;
    private boolean photoIsTaking = false;
    private RadioButton rdBtnOnlyForSell;
    private static final int imageViewDefaultWidth = 300;
    private static final int imageViewDefaultHeight = 230;
    private static final String keyAdvertisementTitleText = "advertisementTitleText";
    private static final String keyRdBtnOnlyForSellValue = "rdBtnOnlyForSellValue";
    private static final String keyAdvertisementDescriptionText = "advertisementDescriptionText";
    private static final String keyAdvertisementPriceValue = "advertisementPriceValue";
    private static final String keyPhotoView1Path = "mPhotoView1Path";
    private static final String keyPhotoIsTakingValue = "keyPhotoIsTakingValue";


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_add_new_advertisement_item);
        gps = new GpsLocationService(this);

        setViewFields(savedInstanceState);
    }

    private void setViewFields(Bundle savedInstanceState) {
        rdBtnOnlyForSell = (RadioButton) findViewById(R.id.rdBtnOnlyForSell);
        progress = new ProgressDialog(this);
        advertisementTitle = (EditText) findViewById(R.id.editTextTitle);
        advertisementDescription = (EditText) findViewById(R.id.editTextDescription);
        advertisementPrice = (EditText) findViewById(R.id.editTextPrice);
        mPhotoView1 = (ImageView) findViewById(R.id.photoView1);
        mButtonTakePicture = (Button) findViewById(R.id.buttonTakePicture);
        mButtonTakePicture.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                dispatchTakePictureIntent();
            }
        });
        Button mButtonPublishAdvertisementItem = (Button) findViewById(R.id.buttonPublishAdvertisemenetItem);
        mButtonPublishAdvertisementItem.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                publishAdvertisement();
            }
        });

        setImageViewsSize();
        if (savedInstanceState != null) {
            restoreViewFieldsValues(savedInstanceState);
        }
    }

    private void setImageViewsSize() {
        Point screenSize = new Point();
        getWindowManager().getDefaultDisplay().getSize(screenSize);
        int width = screenSize.x;
        int result = (width - 20) - imageViewDefaultWidth;
        if (result > 0) {
            mPhotoView1.getLayoutParams().width = imageViewDefaultWidth + result;
            mPhotoView1.getLayoutParams().height = imageViewDefaultHeight + result;
        }

    }

    private void restoreViewFieldsValues(Bundle savedInstanceState) {
        advertisementTitle.setText(savedInstanceState.getString(keyAdvertisementTitleText, AppConstant.EMPTY_STRING));
        advertisementDescription.setText(savedInstanceState.getString(keyAdvertisementDescriptionText, AppConstant.EMPTY_STRING));
        advertisementPrice.setText(savedInstanceState.getString(keyAdvertisementPriceValue, AppConstant.EMPTY_STRING));
        rdBtnOnlyForSell.setChecked(savedInstanceState.getBoolean(keyRdBtnOnlyForSellValue, false));
        mPhotoPath = savedInstanceState.getString(keyPhotoView1Path, null);
        photoIsTaking = savedInstanceState.getBoolean(keyPhotoIsTakingValue, true);
        if (mPhotoPath != null && !photoIsTaking) {
            setPhoto();
        }
    }

    private void publishAdvertisement() {
        // if(advertisementItemDataIsValidate()){
        //wysyłanko
        showProgressBar();

        // Check if GPS enabled
        if (gps.canGetLocation()) {
            if (gps.getLongitude() != 0.0 && gps.getLatitude() != 0.0) {
                AdvertisementItemsService.getInstance().UploadNewAdvertisementPhotos(mPhotoPath, this);
            } else {
                //lokalizacja jest chujowa
                Toast.makeText(getApplicationContext(), "Wspólrzędne lokalizacji są zerowe", Toast.LENGTH_LONG).show();
            }

        } else {
            // Can't get location.
            // GPS or network is not enabled.
            // Ask user to enable GPS/network in settings.
            gps.showSettingsAlert();
        }
    }

    private void showProgressBar() {
        progress.setMessage("Wysyłanie ogloszenia. Proszę czekać...");
        progress.setProgressStyle(ProgressDialog.STYLE_SPINNER);
        progress.setIndeterminate(false);
        progress.setProgress(99);
        progress.show();
    }

    private NewAdvertisementItem createNewAdvertisementItemModel(ArrayList<String> photosPtahs) {
        NewAdvertisementItem model = new NewAdvertisementItem();
        model.AdvertisementTitle = advertisementTitle.getText().toString();
        model.AdvertisementDescription = advertisementDescription.getText().toString();
        model.Latitude = gps.getLatitude();
        model.Longitude = gps.getLongitude();
        model.IsOnlyForSell = rdBtnOnlyForSell.isChecked();
        model.AdvertisementPrice = Integer.parseInt(advertisementPrice.getText().toString());
        model.PhotosPaths = photosPtahs;

        return model;
    }

    private String getFileExtension(String filePath) {
        try {
            return filePath.substring(filePath.lastIndexOf(".") + 1);
        } catch (Exception e) {
            return "";
        }
    }

    private boolean advertisementItemDataIsValidate() {
        boolean isValidate = true;
        if (advertisementTitle.getText().length() < 20) {
            isValidate = false;
            advertisementTitle.setError("Tytuł musi zawierać min. 15 znaków");
            focusView = advertisementTitle;
        } else if (advertisementDescription.getText().length() < 20) {
            isValidate = false;
            advertisementDescription.setError("Opis musi zawierać min 20 znaków");
            focusView = advertisementDescription;
        } else if (advertisementPrice.getText().length() == 0) {
            isValidate = false;
            advertisementPrice.setError("Cena musi zostać podana");
            focusView = advertisementPrice;
        } else if (mPhotoPath == null) {
            Toast.makeText(this, "Nie dodano żadnego zdjęcia", Toast.LENGTH_LONG).show();
            isValidate = false;
        }
        return isValidate;
    }

    private void dispatchTakePictureIntent() {
        Intent takePictureIntent = new Intent(MediaStore.ACTION_IMAGE_CAPTURE);
        // Ensure that there's a camera activity to handle the intent
        if (takePictureIntent.resolveActivity(getPackageManager()) != null) {
            // Create the File where the photo should go
            File photoFile = null;
            try {
                photoFile = createImageFile();
            } catch (IOException ex) {
                // Error occurred while creating the File
                Toast.makeText(this, "Coś sie zjebało ze zdjeciem", Toast.LENGTH_LONG).show();
            }
            // Continue only if the File was successfully created
            if (photoFile != null) {
                photoIsTaking = true;
                takePictureIntent.putExtra(MediaStore.EXTRA_OUTPUT,
                        Uri.fromFile(photoFile));
                startActivityForResult(takePictureIntent, AppConstant.REQUEST_TAKE_PHOTO);
            }
        }
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        if (requestCode == AppConstant.REQUEST_TAKE_PHOTO) {
            setPhoto();
            photoIsTaking = false;
        }
    }

    private void setPhoto() {
        photoIsTaking = true;
        int targetW = mPhotoView1.getWidth();
        int targetH = mPhotoView1.getHeight();
        Bitmap resizedImage = BitmapOperationService.ResizeImage(mPhotoPath, targetW, targetH);
        mPhotoView1.setImageBitmap(resizedImage);
        mButtonTakePicture.setText("Zrób inne zdjęcie");
        photoIsTaking = false;
    }


    private File createImageFile() throws IOException {
        // Create an image file name
        String timeStamp = new SimpleDateFormat("yyyyMMdd_HHmmss").format(new Date());
        String imageFileName = "JPEG_" + timeStamp + "_";
        File storageDir = Environment.getExternalStoragePublicDirectory(
                Environment.DIRECTORY_PICTURES);

        File image = File.createTempFile(
                imageFileName,  /* prefix */
                ".jpg",         /* suffix */
                storageDir      /* directory */
        );

        // Save a file: path for use with ACTION_VIEW intents
        mCurrentPhotoPath = "file:" + image.getAbsolutePath();
        mPhotoPath = image.getAbsolutePath();
        return image;
    }

    @Override
    public void onAddNewAdvertisementItemPhotosRequestFinished(int statusCode, JSONObject response) {
        if (statusCode == 200) {
            //wszystko gicior
            Toast.makeText(this, "Zdjecia zapisane", Toast.LENGTH_LONG).show();
            AdvertisementItemPhotosPaths advertisementItemPhotosPaths = new Gson().fromJson(response.toString(), AdvertisementItemPhotosPaths.class);

            createAdvertisementItem(advertisementItemPhotosPaths);
        } else if (response != null) {
            ErrorResponse errorResponse = new Gson().fromJson(response.toString(), ErrorResponse.class);
            Toast.makeText(this, errorResponse.ErrorMessage, Toast.LENGTH_LONG).show();
        } else {
            Toast.makeText(this, "Coś poszło nie tak", Toast.LENGTH_LONG).show();
        }
    }

    private void createAdvertisementItem(AdvertisementItemPhotosPaths advertisementItemPhotosPaths) {
        try {
            NewAdvertisementItem model = createNewAdvertisementItemModel(advertisementItemPhotosPaths.PhotosPaths);
            AdvertisementItemsService.getInstance().CreateNewAdvertisementItem(model, this, this);
        } catch (Exception exc) {
            onDataReceived(500, new JSONObject());
        }
    }

    @Override
    public void onDataReceived(int statusCode, JSONObject response) {
        progress.hide();
        if (statusCode == 200) {
            //wszystko gicior
            Toast.makeText(this, "Ogłoszenie zapisane", Toast.LENGTH_LONG).show();
        } else if (response != null) {
            ErrorResponse errorResponse = new Gson().fromJson(response.toString(), ErrorResponse.class);
            Toast.makeText(this, errorResponse.ErrorMessage, Toast.LENGTH_LONG).show();
        } else {
            Toast.makeText(this, "Coś poszło nie tak", Toast.LENGTH_LONG).show();
        }
    }

    @Override
    public void onDataReceived(int statusCode, JSONArray response) {

    }


    @Override
    protected void onSaveInstanceState(Bundle outState) {
        super.onSaveInstanceState(outState);
        saveViewFieldsValues(outState);
    }

    private void saveViewFieldsValues(Bundle outState) {
        outState.putAll(outState);
        outState.putString(keyAdvertisementTitleText, advertisementTitle.getText().toString());
        outState.putString(keyAdvertisementDescriptionText, advertisementDescription.getText().toString());
        outState.putBoolean(keyRdBtnOnlyForSellValue, rdBtnOnlyForSell.isChecked());
        outState.putString(keyAdvertisementPriceValue, advertisementPrice.getText().toString());
        outState.putString(keyPhotoView1Path, mPhotoPath);
        outState.putBoolean(keyPhotoIsTakingValue, photoIsTaking);
    }
}

