package marcin_szyszka.mobileseconndhand.activities;

import android.app.ProgressDialog;
import android.content.Context;
import android.content.Intent;
import android.support.design.widget.FloatingActionButton;
import android.support.v4.app.FragmentActivity;
import android.os.Bundle;
import android.view.View;
import android.widget.Toast;

import marcin_szyszka.mobileseconndhand.R;
import marcin_szyszka.mobileseconndhand.models.AdvertisementItemShortModel;

public class MainActivity extends FragmentActivity implements AdvertisementItemFragment.OnListFragmentInteractionListener {


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        FloatingActionButton addNewItemButton = (FloatingActionButton) findViewById(R.id.fab);
        addNewItemButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                goToAddNewItemActivity();
            }
        });
    }

    private void goToAddNewItemActivity() {
        Intent addNewItemIntent = new Intent(this, AddNewAdvertisementItemActivity.class);
        startActivity(addNewItemIntent);
    }

    @Override
    public void onListFragmentInteraction(AdvertisementItemShortModel item) {
        Toast.makeText(this, "Bum", Toast.LENGTH_LONG).show();
    }

}
