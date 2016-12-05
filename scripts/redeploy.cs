//@req(token, action)

if (token == "${TOKEN}") {
  var targetEnv = "${TARGET_ENV}", nodeGroup = "${NODE_GROUP}";
  if (action == 'redeploy'){
    var delay = getParam("delay") || 30;
    return jelastic.env.control.RestartContainersByGroup(targetEnv, signature, nodeGroup, delay); 
  } else if (action == 'rebuild'){
    var buildEnv = "${BUILD_ENV}", nodeId = "${BUILD_NODE_ID}", projectId = "${PROJECT_ID}"; 
    return jelastic.env.build.BuildProject(buildEnv, signature, nodeId, projectId);
  } else {
    return {"result": 3, "error": "unknown action [" + action + "]"}
  }
} else {
  return {"result": 8, "error": "wrong token"}
}
