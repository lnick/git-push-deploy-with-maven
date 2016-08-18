//@auth
//@required('url', 'scriptName', 'scriptType')

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.net.HttpURLConnection;
import java.net.URL;
import java.util.Collection;
import com.hivext.api.server.development.response.ApplicationInfoResponse;
import com.hivext.api.utils.Random;


//reading script from URL
int READ_TIMEOUT = 30 * 1000;
int CONNECT_TIMEOUT = 10 * 1000;
int BUFFER_SIZE = 2048;

String url = getParam("url");

HttpURLConnection conn = null;
InputStream cis = null;
ByteArrayOutputStream out = null;
try {
    conn = (HttpURLConnection) new URL(url).openConnection();
    conn.setInstanceFollowRedirects(false);
    conn.setReadTimeout(CONNECT_TIMEOUT);
    conn.setConnectTimeout(READ_TIMEOUT);

    byte[] buff = new byte[BUFFER_SIZE];
    cis = conn.getInputStream();
    out = new ByteArrayOutputStream();
    int n;
    while ((n = cis.read(buff)) > -1) {
        out.write(buff, 0, n);
    }
} finally {
    if (cis != null) {
        try {
            cis.close();
        } catch (IOException ex) {}
    }
    if (out != null) {
        try {
            out.close();
        } catch (IOException ex) {}
    }

    if (conn != null) {
        conn.disconnect();
    }

}

//inject token
String token = Random.getPswd(64);
String scriptBody = out.toString("UTF-8").replace("${TOKEN}", token);

//creating a new script 
return hivext.dev.scripting.CreateScript(getParam("scriptName"), getParam("scriptType"), scriptBody);