//@auth
//@req(url, next, targetEnv, nodeGroup)

import com.hivext.api.core.utils.Transport;
import com.hivext.api.utils.Random;

//reading script from URL
var scriptBody = new Transport().get(url)

//inject token
var token = Random.getPswd(64);
scriptBody = scriptBody.replace("${TOKEN}", token);

targetEnv = targetEnv.toString().split(".")[0];
scriptBody = scriptBody.replace("${TARGET_ENV}", targetEnv);
scriptBody = scriptBody.replace("${NODE_GROUP}", nodeGroup.toString());
scriptBody = scriptBody.replace("${BUILD_NODE_ID}", "${nodes.build.first.id}");

var projectId = parseInt("${nodes.build.first.customitem.projects[0].id}", 10);
if (isNaN(projectId)) {
   projectId = jelastic.env.control.GetEnvInfo('${env.envName}', session).nodes[0].customitem.projects[0].id;
}
scriptBody = scriptBody.replace("${PROJECT_ID}", projectId.toString());

//create a new script 
var scriptName = "${env.envName}-${globals.scriptName}"; 

//delete the script if it exists already
jelastic.dev.scripting.DeleteScript(scriptName);

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
