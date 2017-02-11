//@req(token, action)
if (token == "${TOKEN}") {
    var targetEnv = "${TARGET_ENV}",
        nodeGroup = "${NODE_GROUP}", 
        buildNodeId = "${BUILD_NODE_ID}",
        envName = "${BUILD_ENV}",
        projectName = "${PROJECT_NAME}",
        delay = getParam("delay") || 30,
        certified = ${CERTIFIED};

    if (action == 'redeploy') {


        if (certified) {
            var port = 8080;
            var dir = "/var/lib/jelastic/PROJECTS/" + projectName + "/target"
            var cmd = "cd " + dir + "; pkill -f SimpleHTTPServer; screen -md python -m SimpleHTTPServer " + port + ";";
            cmd += " ls -al " + dir + " | grep '^-' | head -n 1 | awk '{print $9}'";

            var resp = jelastic.env.control.ExecCmdById(envName, signature, "build", toJSON([{
                "command": cmd
            }]) + "", true, "root");
            if (resp.result != 0) return resp;

            var fileName = resp.responses[0].out;
            var arr = fileName.split(".");
            arr = arr.pop();
            var context = arr.join(".");
            var url = "http://node" + buildNodeId +"-" + envName + ":" + port + "/" + fileName;
            var respDeploy = jelastic.env.control.DeployApp(targetEnv, signature, url, fileName, context, false, delay);
            if (respDeploy.result != 0) return respDeploy;

            cmd = "pkill -f SimpleHTTPServer";
            resp = jelastic.env.control.ExecCmdById(envName, signature, buildNodeId, toJSON([{
                "command": cmd
            }]), true, "root");
            if (resp.result != 0) return resp;
          
            return respDeploy;
        } else {
            return jelastic.env.control.RestartContainersByGroup(targetEnv, signature, nodeGroup, delay);
        }
    } else if (action == 'rebuild') {
        var buildEnv = "${BUILD_ENV}",
            nodeId = "${BUILD_NODE_ID}",
            projectId = "${PROJECT_ID}";
        return jelastic.env.build.BuildProject(buildEnv, signature, nodeId, projectId);
    } else {
        return {
            "result": 3,
            "error": "unknown action [" + action + "]"
        }
    }
} else {
    return {
        "result": 8,
        "error": "wrong token"
    }
}
