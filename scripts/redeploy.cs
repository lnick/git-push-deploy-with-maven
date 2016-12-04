//@required('token', 'targetEnv', 'nodeGroup')

if (token == "${TOKEN}") {
  return jelastic.env.control.RestartContainersByGroup(targetEnv, signature, nodeGroup); 
} else {
  return {"result": 8, "error": "wrong token"}
}
