//@req(mountAction)
 
var pathTo = "${settings.deployPathCustom}";
if ("${settings.deployPath}" == "auto") {
    
    //looking for first node in cp layer 
    var group = "cp";
    var nodeId = -1; 
    var envName = "${settings.targetEnv}".split(".")[0];
    var resp = jelastic.env.control.GetEnvInfo(envName, session);
    if (resp.result != 0) return resp;
    var nodes = resp.nodes;
    for (var i = 0; i < nodes.length; i++){
        if (nodes[i].nodeGroup == group) {
            nodeId = nodes[i].id;
            break; 
        }
    }
    if (nodeId == -1){
        return {result: 99, error: "zero nodes in group [" + group + "]"}
    }

    //getting WEBROOT as deployment path
    var cmd = "m=/etc/jelastic/metainf.conf; e=/etc/jelastic/environment; l=/var/lib/jelastic/libs/envinfo.lib; [ -f $m ] && { source $m; [ -f $e ] && source $e || source $l; }; echo ${WEBROOT:-$Webroot_Path}";
    var resp = jelastic.env.control.ExecCmdById(envName, session, nodeId, toJSON([{
        "command": cmd
    }]), true, "root");
    if (resp.result != 0) return resp;
    pathTo = resp.responses[0].out;
    pathTo = pathTo.replace(/^\s+|\s+$/g, "");
    if (!pathTo)
        return {
            result: 99,
            error: "Can't detect the deployment folder"
        }
}

resp.onAfterReturn = {};
resp.onAfterReturn[mountAction] = {pathTo: pathTo};
return resp;
