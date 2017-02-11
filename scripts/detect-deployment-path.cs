var cmd = "m=/etc/jelastic/metainf.conf; e=/etc/jelastic/environment; l=/var/lib/jelastic/libs/envinfo.lib; [ -f $m ] && { source $m; [ -f $e ] && source $e || source $l; }; echo ${WEBROOT:-$Webroot_Path}";

var envName = "${settings.targetEnv}".split(".")[0];
var id = "${nodes.cp.first.id}";

var pathTo = "${settings.deployPathCustom}";
if ("${settings.deployPath}" == "auto") {
    var resp = jelastic.env.control.ExecCmdById(envName, session, id, toJSON([{
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


return {
    result: 0,
    onAfterReturn: {
        "mount-deploy-volume": {
            pathTo: pathTo
        }
    }
}
