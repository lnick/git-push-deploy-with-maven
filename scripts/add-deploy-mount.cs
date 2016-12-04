//@auth 
//@req(pathFrom, pathTo)

var mountFrom = "${nodes.build.first.id}";
var envName = "${settings.targetEnv}".split(".")[0];
var groups = jelastic.env.control.GetEnvInfo(envName, session).nodeGroups; 
var mountTo = "cp";
for (var i = 0; i < groups.length; i++){
  if (groups[i].name == "storage") {
    mountTo = "storage";
    break;
  }
}

//resp = jelastic.env.control.AddDockerVolumeByGroup('${env.envName}', session, mountTo, volume); 
resp = jelastic.env.file.AddMountPointByGroup(envName, session, mountTo, pathTo, 'nfs', null, pathFrom, mountFrom, '', false); 
return resp;
