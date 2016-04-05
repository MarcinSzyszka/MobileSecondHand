package marcin_szyszka.mobileseconndhand.activities;

import android.app.ProgressDialog;
import android.content.Intent;
import android.support.design.widget.FloatingActionButton;
import android.support.v4.app.FragmentActivity;
import android.os.Bundle;
import android.view.View;
import android.widget.ImageButton;
import android.widget.Toast;

import java.io.UnsupportedEncodingException;

import marcin_szyszka.mobileseconndhand.R;
import marcin_szyszka.mobileseconndhand.common.ProgressDialogAsync;
import marcin_szyszka.mobileseconndhand.models.AdvertisementItemShortModel;

public class MainActivity extends FragmentActivity implements AdvertisementItemFragment.OnListFragmentInteractionListener {
    AdvertisementItemFragment mAdvertisementFragment;
    ProgressDialogAsync progressDialogAsync;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        mAdvertisementFragment = (AdvertisementItemFragment) getSupportFragmentManager().findFragmentById(R.id.fragment);
        FloatingActionButton addNewItemButton = (FloatingActionButton) findViewById(R.id.fab);
        addNewItemButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                goToAddNewItemActivity();
            }
        });
        ImageButton getAdvertisementsBtn = (ImageButton) findViewById(R.id.btnRefreshAdvertisementsList);
        getAdvertisementsBtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                try {
                    refreshAdvertisementsList();
                } catch (UnsupportedEncodingException e) {
                    e.printStackTrace();
                }
            }
        });

    }

    private void refreshAdvertisementsList() throws UnsupportedEncodingException {
        mAdvertisementFragment.getAdvertisements();
    }

    private void goToAddNewItemActivity() {
        Intent addNewItemIntent = new Intent(this, AddNewAdvertisementItemActivity.class);
        startActivity(addNewItemIntent);
    }

    @Override
    public void onListFragmentInteraction(AdvertisementItemShortModel item) {
        Toast.makeText(this, "Bum", Toast.LENGTH_LONG).show();
    }

    @Override
    public void onInfinityScroll() throws UnsupportedEncodingException {
        mAdvertisementFragment.getAdvertisements();
    }

    public void onPreparedData() {
        progressDialogAsync.stop();
    }

    @Override
    public void onStartDownloading() {
        progressDialogAsync = new ProgressDialogAsync(this, "Pobieranie ogłoszeń");
        progressDialogAsync.execute();
    }

}
