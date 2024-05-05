export const LIDARR_SET_CLIENT = "musicClients:set_lidarr_client";
export const LIDARR_LOAD_PATHS = "musicClients:load_lidarr_paths";
export const LIDARR_SET_PATHS= "musicClients:set_lidarr_paths";
export const LIDARR_LOAD_PROFILES = "musicClients:load_lidarr_profiles";
export const LIDARR_SET_PROFILES = "musicClients:set_lidarr_profiles";
export const LIDARR_LOAD_TAGS = "musicClients:load_lidarr_tags";
export const LIDARR_SET_TAGS = "musicClients:set_lidarr_tags";


export function setLidarrClient(settings) {
    return {
        type: LIDARR_SET_CLIENT,
        payload: settings
    };
};


export function isLoadingLidarrPaths(isLoading) {
    return {
        type: LIDARR_LOAD_PATHS,
        payload: isLoading
    };
};


export function setLidarrPaths(lidarrPaths) {
    return {
        type: LIDARR_SET_PATHS,
        payload: lidarrPaths
    };
};


export function isLoadingLidarrProfiles(isLoading) {
    return {
        type: LIDARR_LOAD_PROFILES,
        payload: isLoading
    };
};


export function setLidarrProfiles(lidarrProfiles) {
    return {
        type: LIDARR_SET_PROFILES,
        payload: lidarrProfiles
    };
};


export function isLoadingLidarrTags(isLoading) {
    return {
        type: LIDARR_LOAD_TAGS,
        payload: isLoading
    };
};


export function setLidarrTags(lidarrTags) {
    return {
        type: LIDARR_SET_TAGS,
        payload: lidarrTags
    };
};


export function loadLidarrRootPaths(forceReload) {
    return (dispatch, getState) => {
        const state = getState();

        var lidarr = state.music.lidarr;

        if ((!lidarr.hasLoadedPaths && !lidarr.isLoadingPaths) || forceReload) {
            dispatch(isLoadingLidarrPaths(true));

            return fetch("../api/music/lidarr/rootpath", {
                method: 'POST',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${state.user.token}`
                },
                body: JSON.stringify({
                    "Hostname": lidarr.hostname,
                    'BaseUrl': lidarr.baseUrl,
                    "Port": Number(lidarr.port),
                    "ApiKey": lidarr.apiKey,
                    "UseSSL": lidarr.useSSL,
                    "Version": lidarr.version,
                })
            })
                .then(data => {
                    if (data.status !== 200) {
                        throw new Error("Bad request.");
                    }

                    return data;
                })
                .then(data => data.json())
                .then(data => {
                    dispatch(setLidarrPaths(data));

                    return {
                        ok: true,
                        paths: data
                    }
                })
                .catch(err => {
                    dispatch(setLidarrPaths([]));
                    return { ok: false };
                })
        }
        else {
            return new Promise((resolve, reject) => {
                return { ok: false };
            });
        }
    };
};


export function loadLidarrProfiles(forceReload) {
    return (dispatch, getState) => {
        const state = getState();
        var lidarr = state.music.lidarr;

        if ((!lidarr.hasLoadedProfiles && !lidarr.isLoadingProfiles) || forceReload) {
            dispatch(isLoadingLidarrProfiles(true));

            return fetch("../api/music/lidarr/profile", {
                method: 'POST',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${state.user.token}`
                },
                body: JSON.stringify({
                    "Hostname": lidarr.hostname,
                    'BaseUrl': lidarr.baseUrl,
                    "Port": Number(lidarr.port),
                    "ApiKey": lidarr.apiKey,
                    "UseSSL": lidarr.useSSL,
                    "Version": lidarr.version,
                })
            })
                .then(data => {
                    if (data.status !== 200) {
                        throw new Error("Bad request.");
                    }

                    return data;
                })
                .then(data => data.json())
                .then(data => {
                    dispatch(setLidarrProfiles(data));

                    return {
                        ok: true,
                        profiles: data
                    }
                })
                .catch(err => {
                    dispatch(setLidarrProfiles([]));
                    return { ok: false };
                })
        } else {
            return new Promise((resolve, reject) => {
                return { ok: false };
            });
        }
    };
};


export function loadLidarrTags(forceReload) {
    return (dispatch, getState) => {
        const state = getState();

        var lidarr = state.music.lidarr;

        if ((!lidarr.hasLoadedTags && !lidarr.isLoadingTags) || forceReload) {
            dispatch(isLoadingLidarrTags(true));

            return fetch("../api/music/lidarr/tag", {
                method: 'POST',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${state.user.token}`
                },
                body: JSON.stringify({
                    "Hostname": lidarr.hostname,
                    'BaseUrl': lidarr.baseUrl,
                    "Port": Number(lidarr.port),
                    "ApiKey": lidarr.apiKey,
                    "UseSSL": lidarr.useSSL,
                    "Version": lidarr.version,
                })
            })
                .then(data => {
                    if (data.status !== 200) {
                        throw new Error("Bad request.");
                    }

                    return data;
                })
                .then(data => data.json())
                .then(data => {
                    dispatch(setLidarrTags({ ok: true, data: data }));

                    return {
                        ok: true,
                        tags: data
                    }
                })
                .catch(err => {
                    dispatch(setLidarrTags({ ok: false, data: [] }));
                    return { ok: false };
                })
        }
        else {
            return new Promise((resolve, reject) => {
                return { ok: false };
            });
        }
    };
};


export function testLidarrSettings(settings) {
    return (dispatch, getState) => {
        const state = getState();

        return fetch("../api/music/lidarr/test", {
            method: "POST",
            headers: {
                "Accept": "application/json",
                "Content-Type": "application/json",
                "Authorization": `Bearer ${state.user.token}`
            },
            body: JSON.stringify({
                "Hostname": settings.hostname,
                "BaseUrl": settings.baseUrl,
                "Port": Number(settings.port),
                "ApiKey": settings.apiKey,
                "UseSSL": settings.useSSL,
                "Version": settings.version
            })
        })
            .then(data => data.json())
            .then(data => {
                dispatch(loadLidarrProfiles(true));
                dispatch(loadLidarrRootPaths(true));
                dispatch(loadLidarrTags(true));

                if (data.ok)
                    return { ok: true };
                return { ok: false, error: data };
            });
    }
}