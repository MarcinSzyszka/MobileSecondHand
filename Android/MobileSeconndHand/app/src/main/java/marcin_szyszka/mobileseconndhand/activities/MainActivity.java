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
import marcin_szyszka.mobileseconndhand.models.AdvertisementItemShortModel;

public class MainActivity extends FragmentActivity implements AdvertisementItemFragment.OnListFragmentInteractionListener {
    AdvertisementItemFragment mAdvertisementFragment;
    private ProgressDialog progress;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        progress = new ProgressDialog(this);
        showProgressBar();
        mAdvertisementFragment = (AdvertisementItemFragment) getSupportFragmentManager().findFragmentById(R.id.fragment);
        mAdvertisementFragment.setListener(this);
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

    public void onPreparedData() {
        progress.hide();
    }
    private void showProgressBar() {
        progress.setMessage("Pobieram ogłoszenia...");
        progress.setProgressStyle(ProgressDialog.STYLE_SPINNER);
        progress.setIndeterminate(false);
        progress.setProgress(99);
        progress.show();
    }

}
