//@req(user, repo, token, callbackUrl, scriptName)

import org.apache.commons.httpclient.HttpClient;
import org.apache.commons.httpclient.methods.GetMethod;
import org.apache.commons.httpclient.methods.DeleteMethod;
import org.apache.commons.httpclient.methods.PostMethod;
import org.apache.commons.httpclient.UsernamePasswordCredentials;
import org.apache.commons.httpclient.methods.StringRequestEntity;
import org.apache.commons.httpclient.auth.AuthScope;
import com.hivext.api.core.utils.JSONUtils;
import java.io.InputStreamReader;
import java.io.BufferedReader;

var client = new HttpClient();

//Authentication
var creds = new UsernamePasswordCredentials(user, token);
client.getParams().setAuthenticationPreemptive(true);
client.getState().setCredentials(AuthScope.ANY, creds);

//Parsing repo url
var domain = "github.com";
if (repo.indexOf(".git") > -1) repo = repo.split(".git")[0];
if (repo.indexOf("/") > -1) {
    var arr = repo.split("/");
    repo = arr.pop();
    if (repo == "") repo = arr.pop();
    user = arr.pop();
    domain = arr.pop() || domain;
}

//Get list of hooks
var get = new GetMethod("https://api." + domain + "/repos/" + user + "/" + repo + "/hooks");
var hooks = eval("(" + exec(get) + ")");

//Clear previous hooks
for (var i = 0; i < hooks.length; i++) {
    if (hooks[i].config.url.indexOf(scriptName) != -1) {
        var del = new DeleteMethod("https://api." + domain + "/repos/" + user + "/" + repo + "/hooks/" + hooks[i].id);
        exec(del);
    }
}

var action = getParam('act');
if (action == 'delete' || action == 'clean'){
    return {result:0};
}

//Create a new hook
var post = new PostMethod("https://api." + domain + "/repos/" + user + "/" + repo + "/hooks");

//Hook request params
var params = {
    "name": "web",
    "active": true,
    "events": ["push", "pull_request"],
    "config": {
        "url": callbackUrl,
        "content_type": "json"
    }
};

var newHook = eval("(" + exec(post, params) + ")");
return {
    result: 0, 
    hook: newHook
};

function exec(method, params) {
    if (params) {
        var requestEntity = new StringRequestEntity(JSONUtils.jsonStringify(params), "application/json", "UTF-8");
        method.setRequestEntity(requestEntity);
    }
    var status = client.executeMethod(method),
        response = "";
    if (status == 200 || status == 201) {
        var br = new BufferedReader(new InputStreamReader(method.getResponseBodyAsStream())),
            line;
        while ((line = br.readLine()) != null) {
            response = response + line;
        }
    }
    method.releaseConnection();
    return response;
}

