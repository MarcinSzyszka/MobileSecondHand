package marcin_szyszka.mobileseconndhand.activities;

import android.support.v4.app.FragmentActivity;
import android.os.Bundle;
import android.widget.Toast;

import marcin_szyszka.mobileseconndhand.R;
import marcin_szyszka.mobileseconndhand.dummy.DummyContent;

public class MainActivity extends FragmentActivity implements ItemFragment.OnListFragmentInteractionListener {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
    }

    @Override
    public void onListFragmentInteraction(DummyContent.DummyItem item) {
        Toast.makeText(this, "Bum", Toast.LENGTH_LONG).show();
    }
}
