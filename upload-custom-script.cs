//@auth
//@required('url', 'next')

import com.hivext.api.core.utils.Transport;
import com.hivext.api.utils.Random;

//reading script from URL
var scriptBody = new Transport().get(url)

//inject token
var token = Random.getPswd(64);
scriptBody = scriptBody.replace("${TOKEN}", token);

//delete the script if it exists already
jelastic.dev.scripting.DeleteScript(scriptName);

//create a new script 
var envName = "${env.envName}",
    scriptName = envName + "-git-push-redeploy"; 
  
var resp = jelastic.dev.scripting.CreateScript(scriptName, 'js', scriptBody);
if (resp.result != 0) return resp;

//get app domain
var domain = jelastic.dev.apps.GetApp(appid).hosting.domain;

return {
    result: 0,
    onAfterReturn : {
        call : {
            procedure : next,
            params : {
                domain : domain,
                token : token
            }
        }
    }
}
