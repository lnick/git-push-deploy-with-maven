//@req(token, action)

if (token == "${TOKEN}") {
  var targetEnv = "${TARGET_ENV}", nodeGroup = "${NODE_GROUP}";
  if (action == 'redeploy'){
    return jelastic.env.control.RestartContainersByGroup(targetEnv, signature, nodeGroup); 
  } else if (action == 'rebuild'){
    var nodeId = "${BUILD_NODE_ID}", projectId = "${PROJECT_ID}"; 
    return jelastic.env.build.BuildProject(targetEnv, signature, nodeId, projectId);
  } else {
    return {"result": 3, "error": "unknown action [" + action + "]"}
  }
} else {
  return {"result": 8, "error": "wrong token"}
}
