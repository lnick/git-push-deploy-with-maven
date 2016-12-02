//@url('/redeploy') 
//@required('token', 'targetAppid', 'nodeGroup')

if (token == "${TOKEN}") {
  return jelastic.env.control.RestartContainersByGroup(targetAppid, signature, nodeGroup); 
} else {
  return {"result": 8, "error": "wrong token"}
}
