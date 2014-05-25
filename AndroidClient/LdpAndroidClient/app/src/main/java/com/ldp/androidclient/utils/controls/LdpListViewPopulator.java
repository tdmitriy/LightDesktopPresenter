package com.ldp.androidclient.utils.controls;

import android.app.Activity;
import android.content.Context;
import android.graphics.drawable.Drawable;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.ImageView;
import android.widget.TextView;

import com.ldp.androidclient.R;
import com.ldp.androidclient.utils.controls.image_view.LdpClickableImageView;
import com.ldp.androidclient.utils.user_preferences.LdpConnectionPreferences;

import java.util.ArrayList;

public class LdpListViewPopulator extends BaseAdapter {


    public View.OnClickListener onClickListener;
    LayoutInflater inflater;
    ViewHolder holder;

    private ArrayList<LdpConnectionPreferences> pref;

    public LdpListViewPopulator(Activity activity, ArrayList<LdpConnectionPreferences> pref) {
        this.pref = pref;
        inflater = (LayoutInflater) activity
                .getSystemService(Context.LAYOUT_INFLATER_SERVICE);
    }

    @Override
    public int getCount() {
        return pref.size();
    }

    @Override
    public Object getItem(int position) {
        return pref.get(position);
    }

    @Override
    public long getItemId(int position) {
        return position;
    }

    @Override
    public View getView(int position, View convertView, ViewGroup parent) {
        View vi = convertView;
        if (convertView == null) {

            vi = inflater.inflate(R.layout.list_view_layout, null);
            holder = new ViewHolder();

            holder.txtConnectionName = (TextView) vi.findViewById(R.id.txtConnectionName);
            holder.imgPC = (ImageView) vi.findViewById(R.id.list_image);
            holder.imgSettings = (LdpClickableImageView) vi.findViewById(R.id.imageViewSettings);

            vi.setTag(holder);
        } else {

            holder = (ViewHolder) vi.getTag();
        }

        holder.txtConnectionName.setText(pref.get(position).getConnectionName());
        Drawable imagePC = vi.getContext().getResources().getDrawable(R.drawable.icon_pc);
        Drawable imageSettings = vi.getContext().getResources().getDrawable(R.drawable.ic_action_settings);
        holder.imgPC.setImageDrawable(imagePC);
        holder.imgSettings.setImageDrawable(imageSettings);
        if (onClickListener != null)
            holder.imgSettings.setOnClickListener(onClickListener);
        return vi;
    }

    static class ViewHolder {
        TextView txtConnectionName;
        ImageView imgPC;
        ImageView imgSettings;
    }
}
