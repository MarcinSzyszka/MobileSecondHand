package marcin_szyszka.mobileseconndhand.common;

import android.app.ProgressDialog;
import android.content.Context;
import android.os.AsyncTask;
import android.os.Looper;

/**
 * Created by marcianno on 2016-03-22.
 */
public class ProgressDialogAsync extends AsyncTask<Void, Integer, Void> {

    boolean running;
    ProgressDialog progressDialog;
    Context mContext;
    Boolean stopped;
    String mMessage;

    public ProgressDialogAsync(Context context, String message) {
        mContext = context;
        stopped = false;
        mMessage = message;
    }

    @Override
    protected Void doInBackground(Void... params) {
        int i = 10;
        while (running) {
            try {
                Thread.sleep(100);
            } catch (InterruptedException e) {
                e.printStackTrace();
            }

            if (stopped) {
                running = false;
            }

            //publishProgress(i);

        }
        return null;
    }

    @Override
    protected void onProgressUpdate(Integer... values) {
        super.onProgressUpdate(values);
        //progressDialog.setMessage(String.valueOf(values[0]));

    }

    @Override
    protected void onPreExecute() {
        super.onPreExecute();
        running = true;

        progressDialog = ProgressDialog.show(mContext,
                mMessage,
                "Chileczkę...");
        progressDialog.setProgressStyle(ProgressDialog.STYLE_SPINNER);
        progressDialog.setCanceledOnTouchOutside(false);
    }

    @Override
    protected void onPostExecute(Void aVoid) {
        super.onPostExecute(aVoid);
        progressDialog.dismiss();
    }

    public void stop() {
        stopped = true;
    }
}