node {
    currentBuild.result = "SUCCESS"

    try {
      
        def branch
        stage('Checkout') {
            checkout scm
            b = scm.branches[0]
            bs = b.toString()
            branch = bs.substring(bs.lastIndexOf("/") + 1)
        }
        
        def dockername = "shellApi"
        stage('Test') {
            sh 'echo "Test"'
        }

        stage('Build') {
            sh "docker build http://jenkins:jenkins@github.com/bigmacd/shellApi.git#${branch} -t ${dockername}:${env.BUILD_NUMBER}"
        }
   
        stage('Publish') {
            sh 'echo "Publish"'
        }
        
        stage('Deploy') {
            // jenkins doesn't handle piped shell commands, so eval is the way to go
            // it does support: sh "docker ps -a | grep shellapi | awk '{print$1}'"
            // but not in a way that returns stdout
            def getContainers = $/eval "docker ps -a | grep ${dockername} | awk '{print \$1}'"/$
            isRunning = sh (script: "${getContainers}", returnStdout: true).trim()
            ids = isRunning.split()
            // jenkins doesn't support new style iterators, i.e., for (id in ids)
            for (i=0; i < ids.length; i++) {
                def id = ids[i]
                def stopCommand = "docker stop " + "${id}"
                def removeCommand = "docker rm " + "${id}"
                try {
                    sh "${stopCommand}"
                }
                catch (e) { echo "${e.message}" }
                try {
                    sh "${removeCommand}"
                }
                catch(e) { echo "${e.message}"}
            }
            def getDockerHost = $/eval "ip route | awk '/^default/ { print \$3 }'"/$
            dockerHost = sh (script: "${getDockerHost}", returnStdout: true).trim()
            sh "docker run -e environment=${branch} --add-host=\"dockerhost:${dockerHost}\" -d --name ${dockername} --restart=always -p 10000:80 ${dockername}:${env.BUILD_NUMBER}"
        }
    }
    catch(e) {
       currentBuild.result = "FAILURE"
        mail body: "Build ${env.BUILD_NUMBER} of ${env.JOB_NAME} failed.  Reason: ${e.message}.  Go have a look at Jenkins at ${env.RUN_DISPLAY_URL}",
             from: 'jenkins@github.com',
             replyTo: 'noone@nowhere.com',
             subject: "Build Failed!",
             to: 'me@nowhere.org'
        throw(e)
    }
}