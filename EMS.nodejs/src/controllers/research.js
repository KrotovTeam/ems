const {Users, Requests} = require('../db/index');
const boom = require('boom');

async function create(username, researchName, coordinateUser, coordinateScene, numberYears, pathsDownload, miniImagePath,
                      linksDownload, cloudMax, month, status = 1) {
    // TODO небезопасный метод
    const user = await Users.findOne({where: {username}});

    if (!user) {
        throw boom.notFound('user no found');
    }

    const request = await Requests.create({
        researchName,
        status,
        result: {},
        coordinateUser,
        coordinateScene,
        numberYears,
        pathsDownload,
        miniImagePath,
        linksDownload,
        cloudMax,
        month
    });
    await user.addRequest(request);


    return {
        id: request.id,
        success: true
    };
}



async function setPathsDownload(id, pathsDownload){
    const request = await Requests.findOne({where:{id}});
    if(!request){
        return console.log('request не найден');
    }
    request.pathsDownload = pathsDownload;
    await request.save();

}

async function setMiniImagePath(id, miniImagePath){
    const request = await Requests.findOne({where:{id}});
    if(!request){
        return console.log('request не найден');
    }
    request.miniImagePath = miniImagePath;
    await request.save();
}
async function setLinksDownload(id, linksDownload){
    const request = await Requests.findOne({where:{id}});
    if(!request){
        return console.log('request не найден');
    }
    request.linksDownload = linksDownload;
    await request.save();
}
async function setStatus(id, status){
    const request = await Requests.findOne({where:{id}});
    if(!request){
        return console.log('request не найден');
    }
    request.status = status;
    await request.save();
}



module.exports = {
    create,
    setPathsDownload,
    setMiniImagePath,
    setLinksDownload,
    setStatus,
};
